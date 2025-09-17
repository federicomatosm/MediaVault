using MediaVault.Application.Common;
using MediaVault.Application.Entities;

namespace MediaVault.Application.Services.ProfileImages;

public interface IProfileImageService
{
    Task<Result<IReadOnlyCollection<ProfileImageModel>>> UploadAsync(
        CustomerType ownerType,
        long ownerId,
        IReadOnlyCollection<ProfileImageUploadItem> uploads,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<ProfileImageModel>>> ListAsync(
        CustomerType ownerType,
        long ownerId,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        CustomerType ownerType,
        long ownerId,
        long imageId,
        CancellationToken cancellationToken = default);
}
