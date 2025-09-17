namespace MediaVault.Application.Options;

public sealed class ProfileImageOptions
{
    public int MaxPerProfile { get; init; } = 10;
    public int MaxImageBytes { get; init; } = 3 * 1024 * 1024; 
    public string[] AllowedMimeTypes { get; init; } = ["image/jpeg", "image/png", "image/webp"];
}