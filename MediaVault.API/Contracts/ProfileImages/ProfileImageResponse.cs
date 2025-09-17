namespace MediaVault.API.Contracts.ProfileImages;

public sealed record ProfileImageResponse(
    long Id,
    string MimeType,
    string Base64Data,
    string? OriginalFileName,
    DateTime CreatedUtc,
    int SizeBytes);
