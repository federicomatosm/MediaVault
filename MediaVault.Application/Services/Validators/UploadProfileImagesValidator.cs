using FluentValidation;
using MediaVault.Application.Options;
using MediaVault.Application.Services.ProfileImages;
using Microsoft.Extensions.Options;

namespace MediaVault.Application.Services.Validators;

public sealed class UploadProfileImagesValidator : AbstractValidator<IReadOnlyCollection<ProfileImageUploadItem>>
{
    public UploadProfileImagesValidator(
        IValidator<ProfileImageUploadItem> itemValidator,
        IOptions<ProfileImageOptions> options)
    {
        ArgumentNullException.ThrowIfNull(itemValidator);
        ArgumentNullException.ThrowIfNull(options);

        var profileOptions = options.Value;
        var maxPerProfile = profileOptions.MaxPerProfile;

        RuleFor(x => x)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(ProfileImageErrors.NoImagesProvided.Message)
            .WithErrorCode(ProfileImageErrors.NoImagesProvided.Code)
            .WithState(_ => ProfileImageErrors.NoImagesProvided);

        if (maxPerProfile > 0)
        {
            RuleFor(x => x.Count)
                .LessThanOrEqualTo(maxPerProfile)
                .WithMessage(ProfileImageErrors.ImagesLimitExceeded(maxPerProfile).Message)
                .WithErrorCode(ProfileImageErrors.ImagesLimitExceeded(maxPerProfile).Code)
                .WithState(_ => ProfileImageErrors.ImagesLimitExceeded(maxPerProfile));
        }

        RuleForEach(x => x)
            .SetValidator(itemValidator);
    }
}
