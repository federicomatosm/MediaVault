using System.Collections.Generic;
using MediaVault.Application.Common;

namespace MediaVault.Application.Services.ProfileImages;

public static class ProfileImageErrors
{
    public static Error ProfileNotFound => new("profile.not_found", "The requested customer or lead does not exist.");

    public static Error NoImagesProvided => new("profile_images.none_provided", "At least one image must be supplied.");

    public static Error ImagesLimitExceeded(int maxAllowed) => new(
        "profile_images.limit_exceeded",
        $"A profile can store at most {maxAllowed} images. Remove an image before uploading more.");

    public static Error DuplicateImage(string? originalFileName) => new(
        "profile_images.duplicate",
        originalFileName is { Length: > 0 }
            ? $"An identical image for '{originalFileName}' already exists for this profile."
            : "An identical image already exists for this profile.");

    public static Error ImageTooLarge(string? originalFileName, int maxBytes) => new(
        "profile_images.too_large",
        originalFileName is { Length: > 0 }
            ? $"Image '{originalFileName}' exceeds the maximum allowed size of {maxBytes} bytes."
            : $"An image exceeds the maximum allowed size of {maxBytes} bytes.");

    public static Error ImageNotFound(long imageId) => new(
        "profile_images.not_found",
        $"Image with id {imageId} was not found for the specified profile.");

    public static Error Base64Required => new("profile_images.base64_required", "Base64Data is required.");

    public static Error InvalidBase64 => new("profile_images.invalid_base64", "Base64Data must be a valid Base64 string.");

    public static Error MimeTypeRequired => new("profile_images.mimetype_required", "MimeType is required.");

    public static Error InvalidMimeType(string mimeType, IEnumerable<string> allowedMimeTypes)
    {
        var allowed = string.Join(", ", allowedMimeTypes);
        return new Error(
            "profile_images.invalid_mime_type",
            string.IsNullOrWhiteSpace(mimeType)
                ? $"MimeType is not supported. Allowed types: {allowed}."
                : $"MimeType '{mimeType}' is not supported. Allowed types: {allowed}.");
    }

    public static Error FileNameTooLong(int maxLength) => new(
        "profile_images.filename_too_long",
        $"OriginalFileName must be {maxLength} characters or fewer.");
}
