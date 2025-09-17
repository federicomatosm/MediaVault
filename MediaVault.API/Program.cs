using FluentValidation;
using MediaVault.API.Infrastructure;
using MediaVault.Application;
using MediaVault.Application.Options;
using MediaVault.Application.Services.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<ProfileImageUploadItemValidator>();

builder.Services.Configure<ProfileImageOptions>(builder.Configuration.GetSection("ProfileImage"));

var connectionString = builder.Configuration.GetConnectionString("MediaVaultDb")
    ?? throw new InvalidOperationException("Connection string 'MediaVaultDb' was not found.");

builder.Services.AddDatabase(connectionString);
builder.Services.AddApplicationCore();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
