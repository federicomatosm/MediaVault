namespace MediaVault.Application.Entities;

public sealed class ProfileImage
{
    public long Id { get; init; }
    public CustomerType OwnerType { get; init; }
    public long OwnerId { get; init; }

    public string Base64Data { get; init; } = null!;
    public string MimeType { get; init; } = null!;
    public string? OriginalFileName { get; init; }

    public int ContentBytesSize { get; init; }
    public byte[] ContentHashSha256 { get; init; } = null!;
    public DateTime CreatedUtc { get; init; }
    
    public Customer Owner { get; init; } = null!;
}
