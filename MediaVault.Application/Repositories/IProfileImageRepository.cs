using MediaVault.Application.Entities;

namespace MediaVault.Application.Repositories;

public interface IProfileImageRepository
{
    Task<int> CountByOwnerAsync(CustomerType ownerType, long ownerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProfileImage>> ListByOwnerAsync(CustomerType ownerType, long ownerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<byte[]>> GetContentHashesAsync(CustomerType ownerType, long ownerId, CancellationToken cancellationToken = default);
    Task<ProfileImage?> FindByIdAsync(long id, CustomerType ownerType, long ownerId, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<ProfileImage> images, CancellationToken cancellationToken = default);
    Task RemoveAsync(ProfileImage image);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
