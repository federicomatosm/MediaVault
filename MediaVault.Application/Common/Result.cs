using System.Diagnostics.CodeAnalysis;

namespace MediaVault.Application.Common;

public sealed record Error(string Code, string Message)
{
    public static Error None => new("none", string.Empty);
}

public class Result
{
    protected Result(bool isSuccess, Error? error)
    {
        switch (isSuccess)
        {
            case true when error is not null:
                throw new ArgumentException("Successful result cannot contain an error.", nameof(error));
            case false when error is null:
                throw new ArgumentNullException(nameof(error), "Failure result must contain an error.");
            default:
                IsSuccess = isSuccess;
                Error = error;
                break;
        }
    }

    public bool IsSuccess { get; }
    private bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    public static Result Success() => new(true, null);

    public static Result Failure(string code, string message) => Failure(new Error(code, message));

    public static Result Failure(Error error) => new(false, error);
    
}

public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(T value)
        : base(true, null)
    {
        _value = value;
    }

    private Result(Error error)
        : base(false, error)
    {
        _value = default;
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value when the result is a failure.");

    public static Result<T> Success(T value) => new(value);

    public new static Result<T> Failure(string code, string message) => Failure(new Error(code, message));

    public new static Result<T> Failure(Error error) => new(error);
   
}
