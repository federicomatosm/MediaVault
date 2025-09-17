using System.Security.Cryptography;
using FluentValidation;
using MediaVault.Application.Common;
using MediaVault.Application.Entities;
using MediaVault.Application.Options;
using MediaVault.Application.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaVault.Application.Services.ProfileImages;

public sealed class ProfileImageService : IProfileImageService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProfileImageRepository _profileImageRepository;
    private readonly IOptions<ProfileImageOptions> _options;
    private readonly IValidator<IReadOnlyCollection<ProfileImageUploadItem>> _uploadsValidator;
    private readonly ILogger<ProfileImageService> _logger;

    public ProfileImageService(
        ICustomerRepository customerRepository,
        IProfileImageRepository profileImageRepository,
        IOptions<ProfileImageOptions> options,
        IValidator<IReadOnlyCollection<ProfileImageUploadItem>> uploadsValidator,
        ILogger<ProfileImageService> logger)
    {
        _customerRepository = customerRepository;
        _profileImageRepository = profileImageRepository;
        _options = options;
        _uploadsValidator = uploadsValidator;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyCollection<ProfileImageModel>>> UploadAsync(
        CustomerType ownerType,
        long ownerId,
        IReadOnlyCollection<ProfileImageUploadItem> uploads,
        CancellationToken cancellationToken = default)
    {
        if (uploads is null || uploads.Count == 0)
        {
            return Result<IReadOnlyCollection<ProfileImageModel>>.Failure(ProfileImageErrors.NoImagesProvided);
        }

        if (!await _customerRepository.ExistsAsync(ownerId, ownerType, cancellationToken).ConfigureAwait(false))
        {
            return Result<IReadOnlyCollection<ProfileImageModel>>.Failure(ProfileImageErrors.ProfileNotFound);
        }

        var validationResult = await _uploadsValidator
            .ValidateAsync(uploads, cancellationToken)
            .ConfigureAwait(false);

        if (!validationResult.IsValid)
        {
            var failure = validationResult.Errors.First();
            var error = failure.CustomState as Error
                        ?? (failure.ErrorCode is { Length: > 0 }
                            ? new Error(failure.ErrorCode, failure.ErrorMessage)
                            : new Error("profile_images.invalid_request", failure.ErrorMessage));

            return Result<IReadOnlyCollection<ProfileImageModel>>.Failure(error);
        }

        var maxPerProfile = _options.Value.MaxPerProfile;
        if (maxPerProfile <= 0)
        {
            return Result<IReadOnlyCollection<ProfileImageModel>>.Failure(ProfileImageErrors.ImagesLimitExceeded(0));
        }

        var existingCount = await _profileImageRepository
            .CountByOwnerAsync(ownerType, ownerId, cancellationToken)
            .ConfigureAwait(false);

        if (existingCount >= maxPerProfile || existingCount + uploads.Count > maxPerProfile)
        {
            return Result<IReadOnlyCollection<ProfileImageModel>>.Failure(ProfileImageErrors.ImagesLimitExceeded(maxPerProfile));
        }

        var existingHashes = await _profileImageRepository
            .GetContentHashesAsync(ownerType, ownerId, cancellationToken)
            .ConfigureAwait(false);

        var existingHashSet = new HashSet<string>(existingHashes.Select(Convert.ToBase64String), StringComparer.Ordinal);
        var requestHashSet = new HashSet<string>(StringComparer.Ordinal);

        var imagesToPersist = new List<ProfileImage>(uploads.Count);

        foreach (var upload in uploads)
        {
            byte[] contentBytes;
            try
            {
                contentBytes = Convert.FromBase64String(upload.Base64Data);
            }
            catch (FormatException ex)
            {
                _logger.LogWarning(ex, "Invalid Base64 payload for profile image upload.");
                return Result<IReadOnlyCollection<ProfileImageModel>>.Failure(ProfileImageErrors.InvalidBase64);
            }

            var hashBytes = SHA256.HashData(contentBytes);
            var hashString = Convert.ToBase64String(hashBytes);

            if (!requestHashSet.Add(hashString) || existingHashSet.Contains(hashString))
            {
                return Result<IReadOnlyCollection<ProfileImageModel>>.Failure(
                    ProfileImageErrors.DuplicateImage(upload.OriginalFileName));
            }

            imagesToPersist.Add(new ProfileImage
            {
                OwnerType = ownerType,
                OwnerId = ownerId,
                Base64Data = upload.Base64Data,
                MimeType = upload.MimeType,
                OriginalFileName = upload.OriginalFileName,
                ContentBytesSize = contentBytes.Length,
                ContentHashSha256 = hashBytes,
                CreatedUtc = DateTime.UtcNow
            });
        }

        await _profileImageRepository.AddRangeAsync(imagesToPersist, cancellationToken).ConfigureAwait(false);
        await _profileImageRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var persisted = await _profileImageRepository
            .ListByOwnerAsync(ownerType, ownerId, cancellationToken)
            .ConfigureAwait(false);

        var view = persisted.Select(ToModel).ToList();

        return Result<IReadOnlyCollection<ProfileImageModel>>.Success(view);
    }

    public async Task<Result<IReadOnlyCollection<ProfileImageModel>>> ListAsync(
        CustomerType ownerType,
        long ownerId,
        CancellationToken cancellationToken = default)
    {
        if (!await _customerRepository.ExistsAsync(ownerId, ownerType, cancellationToken).ConfigureAwait(false))
        {
            return Result<IReadOnlyCollection<ProfileImageModel>>.Failure(ProfileImageErrors.ProfileNotFound);
        }

        var images = await _profileImageRepository
            .ListByOwnerAsync(ownerType, ownerId, cancellationToken)
            .ConfigureAwait(false);

        return Result<IReadOnlyCollection<ProfileImageModel>>.Success(images.Select(ToModel).ToList());
    }

    public async Task<Result> DeleteAsync(
        CustomerType ownerType,
        long ownerId,
        long imageId,
        CancellationToken cancellationToken = default)
    {
        if (!await _customerRepository.ExistsAsync(ownerId, ownerType, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure(ProfileImageErrors.ProfileNotFound);
        }

        var image = await _profileImageRepository
            .FindByIdAsync(imageId, ownerType, ownerId, cancellationToken)
            .ConfigureAwait(false);

        if (image is null)
        {
            return Result.Failure(ProfileImageErrors.ImageNotFound(imageId));
        }

        await _profileImageRepository.RemoveAsync(image).ConfigureAwait(false);
        await _profileImageRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }

    private static ProfileImageModel ToModel(ProfileImage image)
    {
        return new ProfileImageModel(
            image.Id,
            image.MimeType,
            image.Base64Data,
            image.OriginalFileName,
            image.CreatedUtc,
            image.ContentBytesSize);
    }
}
