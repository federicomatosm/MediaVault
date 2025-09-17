namespace MediaVault.API.Contracts.ProfileImages;

public sealed class UploadProfileImagesRequest
{
    public List<UploadProfileImageItem> Images { get; init; } = [];
}

public sealed class UploadProfileImageItem
{
    public string Base64Data { get; init; } = string.Empty;
    public string MimeType { get; init; } = string.Empty;
    public string? OriginalFileName { get; init; }
}
