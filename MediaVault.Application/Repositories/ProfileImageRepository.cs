using MediaVault.Application.Database;
using MediaVault.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaVault.Application.Repositories;

public sealed class ProfileImageRepository(MediaVaultContext context) : IProfileImageRepository
{
    public Task<int> CountByOwnerAsync(CustomerType ownerType, long ownerId, CancellationToken cancellationToken = default)
    {
        return context.ProfileImages
            .AsNoTracking()
            .Where(x => x.OwnerType == ownerType && x.OwnerId == ownerId)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProfileImage>> ListByOwnerAsync(CustomerType ownerType, long ownerId, CancellationToken cancellationToken = default)
    {
        var images = await context.ProfileImages
            .AsNoTracking()
            .Where(x => x.OwnerType == ownerType && x.OwnerId == ownerId)
            .OrderBy(x => x.CreatedUtc)
            .ToListAsync(cancellationToken);

        return images;
    }

    public async Task<IReadOnlyList<byte[]>> GetContentHashesAsync(CustomerType ownerType, long ownerId, CancellationToken cancellationToken = default)
    {
        var hashes = await context.ProfileImages
            .AsNoTracking()
            .Where(x => x.OwnerType == ownerType && x.OwnerId == ownerId)
            .Select(x => x.ContentHashSha256)
            .ToListAsync(cancellationToken);

        return hashes;
    }

    public Task<ProfileImage?> FindByIdAsync(long id, CustomerType ownerType, long ownerId, CancellationToken cancellationToken = default)
    {
        return context.ProfileImages
            .FirstOrDefaultAsync(
                x => x.Id == id && x.OwnerType == ownerType && x.OwnerId == ownerId,
                cancellationToken);
    }

    public Task AddRangeAsync(IEnumerable<ProfileImage> images, CancellationToken cancellationToken = default)
    {
        return context.ProfileImages.AddRangeAsync(images, cancellationToken);
    }

    public Task RemoveAsync(ProfileImage image)
    {
        context.ProfileImages.Remove(image);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
