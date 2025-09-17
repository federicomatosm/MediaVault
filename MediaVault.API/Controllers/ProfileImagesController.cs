using MediaVault.API.Contracts.ProfileImages;
using MediaVault.Application.Common;
using MediaVault.Application.Entities;
using MediaVault.Application.Services.ProfileImages;
using Microsoft.AspNetCore.Mvc;

namespace MediaVault.API.Controllers;

[ApiController]
[Route("api/profiles/{profileType}/{profileId:long}/images")]
public sealed class ProfileImagesController : ControllerBase
{
    private readonly IProfileImageService _profileImageService;

    public ProfileImagesController(
        IProfileImageService profileImageService)
    {
        _profileImageService = profileImageService;
    }

    [HttpPost]
    public async Task<IActionResult> UploadAsync(
        string profileType,
        long profileId,
        [FromBody] UploadProfileImagesRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryResolveProfileType(profileType, out var ownerType))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(profileType)] = ["profileType must be either 'customer' or 'lead'."]
            }));
        }
        
        var uploads = request.Images
            .Select(x=> new ProfileImageUploadItem(x.Base64Data, x.MimeType, x.OriginalFileName))
            .ToList();

        var result = await _profileImageService
            .UploadAsync(ownerType, profileId, uploads, cancellationToken)
            .ConfigureAwait(false);

        return MapResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> ListAsync(
        string profileType,
        long profileId,
        CancellationToken cancellationToken)
    {
        if (!TryResolveProfileType(profileType, out var ownerType))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(profileType)] = ["profileType must be either 'customer' or 'lead'."]
            }));
        }

        var result = await _profileImageService
            .ListAsync(ownerType, profileId, cancellationToken)
            .ConfigureAwait(false);

        return MapResult(result);
    }

    [HttpDelete("{imageId:long}")]
    public async Task<IActionResult> DeleteAsync(
        string profileType,
        long profileId,
        long imageId,
        CancellationToken cancellationToken)
    {
        if (!TryResolveProfileType(profileType, out var ownerType))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(profileType)] = ["profileType must be either 'customer' or 'lead'."]
            }));
        }

        var result = await _profileImageService
            .DeleteAsync(ownerType, profileId, imageId, cancellationToken)
            .ConfigureAwait(false);

        return result.IsSuccess ? NoContent() : MapError(result.Error!);
    }

    private IActionResult MapResult(Result<IReadOnlyCollection<ProfileImageModel>> result)
    {
        if (!result.IsSuccess) return MapError(result.Error!);
        var payload = result.Value.Select(ToResponse).ToList();
        return Ok(payload);

    }

    private IActionResult MapError(Error error)
    {
        var status = error.Code switch
        {
            "profile.not_found" => StatusCodes.Status404NotFound,
            "profile_images.not_found" => StatusCodes.Status404NotFound,
            "profile_images.limit_exceeded" => StatusCodes.Status409Conflict,
            "profile_images.duplicate" => StatusCodes.Status409Conflict,
            "profile_images.too_large" => StatusCodes.Status400BadRequest,
            "profile_images.none_provided" => StatusCodes.Status400BadRequest,
            "profile_images.invalid_base64" => StatusCodes.Status400BadRequest,
            "profile_images.base64_required" => StatusCodes.Status400BadRequest,
            "profile_images.mimetype_required" => StatusCodes.Status400BadRequest,
            "profile_images.invalid_mime_type" => StatusCodes.Status400BadRequest,
            "profile_images.filename_too_long" => StatusCodes.Status400BadRequest,
            "profile_images.invalid_request" => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };

        var problem = new ProblemDetails
        {
            Status = status,
            Title = error.Code,
            Detail = error.Message
        };
        problem.Extensions["code"] = error.Code;

        return StatusCode(status, problem);
    }

    private static ProfileImageResponse ToResponse(ProfileImageModel model)
    {
        return new ProfileImageResponse(
            model.Id,
            model.MimeType,
            model.Base64Data,
            model.OriginalFileName,
            model.CreatedUtc,
            model.SizeBytes);
    }

    private static bool TryResolveProfileType(string value, out CustomerType customerType)
    {
        if (string.Equals(value, "customer", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "customers", StringComparison.OrdinalIgnoreCase))
        {
            customerType = CustomerType.Customer;
            return true;
        }

        if (string.Equals(value, "lead", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "leads", StringComparison.OrdinalIgnoreCase))
        {
            customerType = CustomerType.Lead;
            return true;
        }

        customerType = default;
        return false;
    }
}
