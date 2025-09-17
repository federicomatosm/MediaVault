using System.Collections.Generic;
using FluentValidation;
using MediaVault.Application.Database;
using MediaVault.Application.Repositories;
using MediaVault.Application.Services.ProfileImages;
using MediaVault.Application.Services.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MediaVault.Application;

public static class ApplicationServiceCollection
{
    
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MediaVaultContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            });
        });
        return services;
    }

    public static IServiceCollection AddApplicationCore(this IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProfileImageRepository, ProfileImageRepository>();
        services.AddScoped<IProfileImageService, ProfileImageService>();
        services.AddScoped<IValidator<ProfileImageUploadItem>, ProfileImageUploadItemValidator>();
        services.AddScoped<IValidator<IReadOnlyCollection<ProfileImageUploadItem>>, UploadProfileImagesValidator>();
      
        return services;
    }
    
}
