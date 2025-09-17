using FluentValidation;
using MediaVault.Application.Options;
using MediaVault.Application.Services.ProfileImages;
using Microsoft.Extensions.Options;

namespace MediaVault.Application.Services.Validators;

public sealed class ProfileImageUploadItemValidator : AbstractValidator<ProfileImageUploadItem>
{
    public ProfileImageUploadItemValidator(IOptions<ProfileImageOptions> options)
    {
        var profileOptions = options.Value;
        var maxBytes = profileOptions.MaxImageBytes;
        var allowedMimeTypes = profileOptions.AllowedMimeTypes;

        var base64RequiredError = ProfileImageErrors.Base64Required;
        var invalidBase64Error = ProfileImageErrors.InvalidBase64;
        var mimeTypeRequiredError = ProfileImageErrors.MimeTypeRequired;
        var invalidMimeTypeErrorCode = ProfileImageErrors.InvalidMimeType(string.Empty, allowedMimeTypes).Code;
        var fileNameTooLongError = ProfileImageErrors.FileNameTooLong(255);
        var imageTooLargeErrorCode = ProfileImageErrors.ImageTooLarge(null, maxBytes).Code;

        RuleFor(x => x.Base64Data)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(base64RequiredError.Message)
            .WithErrorCode(base64RequiredError.Code)
            .WithState(_ => base64RequiredError)
            .Must(BeValidBase64)
            .WithMessage(invalidBase64Error.Message)
            .WithErrorCode(invalidBase64Error.Code)
            .WithState(_ => invalidBase64Error)
            .Must(value => BeWithinSizeLimit(value, maxBytes))
            .WithMessage(item => ProfileImageErrors.ImageTooLarge(item.OriginalFileName, maxBytes).Message)
            .WithErrorCode(imageTooLargeErrorCode)
            .WithState(item => ProfileImageErrors.ImageTooLarge(item.OriginalFileName, maxBytes));

        RuleFor(x => x.MimeType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(mimeTypeRequiredError.Message)
            .WithErrorCode(mimeTypeRequiredError.Code)
            .WithState(_ => mimeTypeRequiredError)
            .Must(mime => BeAllowedMimeType(mime, allowedMimeTypes))
            .WithMessage(item => ProfileImageErrors.InvalidMimeType(item.MimeType, allowedMimeTypes).Message)
            .WithErrorCode(invalidMimeTypeErrorCode)
            .WithState(item => ProfileImageErrors.InvalidMimeType(item.MimeType, allowedMimeTypes));

        RuleFor(x => x.OriginalFileName)
            .MaximumLength(255)
            .WithMessage(fileNameTooLongError.Message)
            .WithErrorCode(fileNameTooLongError.Code)
            .WithState(_ => fileNameTooLongError);
    }

    private static bool BeValidBase64(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            Convert.FromBase64String(value);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
    private static bool BeWithinSizeLimit(string value, int maxBytes)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            var bytes = Convert.FromBase64String(value);
            return bytes.Length <= maxBytes;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool BeAllowedMimeType(string mimeType, string[] allowedMimeTypes)
    {
        return allowedMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase);
    }
}
