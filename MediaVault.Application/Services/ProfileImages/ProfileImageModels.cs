namespace MediaVault.Application.Services.ProfileImages;

public sealed record ProfileImageUploadItem(string Base64Data, string MimeType, string? OriginalFileName);

public sealed record ProfileImageModel(
    long Id,
    string MimeType,
    string Base64Data,
    string? OriginalFileName,
    DateTime CreatedUtc,
    int SizeBytes);
