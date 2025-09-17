# MediaVault

MediaVault is a .NET 9 Web API that manages customer and lead profiles, allowing each profile to store up to ten Base64-encoded images. The service enforces validation, deduplicates uploads, and exposes CRUD-style endpoints for managing profile image galleries.

## Features
- Customer/lead profile model backed by SQL Server and Entity Framework Core.
- Image uploads persisted as Base64 strings with 10-image cap per profile.
- FluentValidation-driven rules for payloads (Base64 integrity, size limits, MIME types, filename length).
- Result-pattern application layer returning rich error codes for API mapping.
- Global exception handler and ProblemDetails responses for consistent error output.
- Sample REST client requests in `MediaVault.API/MediaVault.API.http`.

## Tech Stack & Libraries
- **.NET 9** Web API
- **Entity Framework Core 9.0.9** (`Microsoft.EntityFrameworkCore`, `SqlServer`, `Design`)
- **FluentValidation 12.0.0** (`FluentValidation`, `FluentValidation.DependencyInjectionExtensions`)
- **Microsoft.AspNetCore.OpenApi 9.0.3** for OpenAPI/Swagger exposure

## Prerequisites
- [.NET SDK 9.0](https://dotnet.microsoft.com/download)
- SQL Server instance (local installation or container)

## Configuration
Update the connection string in `MediaVault.API/appsettings.json` (and `.Development.json`) under `ConnectionStrings:MediaVaultDb` to point at your SQL Server instance:

```json
"ConnectionStrings": {
  "MediaVaultDb": "Server=localhost;Database=MediaVault;User Id=sa;Password=YourPassword;TrustServerCertificate=true;"
}
```

Optionally adjust image constraints in `ProfileImage` settings (max per profile, max bytes, allowed MIME types).

## Getting Started

```bash
# Restore packages
dotnet restore

# Apply database migrations (creates schema if absent)
dotnet ef database update --project MediaVault.Application

# Run the Web API
dotnet run --project MediaVault.API
```

The API defaults to `https://localhost:5001` (`http://localhost:5000`).

## API Endpoints

| Method | URL Pattern | Description |
|--------|-------------|-------------|
| GET | `/api/profiles/{profileType}/{profileId}/images` | List all images for a customer/lead (`profileType` = `customer` or `lead`). |
| POST | `/api/profiles/{profileType}/{profileId}/images` | Upload one or more images (Base64 payload) to a profile; enforces 10 image max, size, and MIME validation. |
| DELETE | `/api/profiles/{profileType}/{profileId}/images/{imageId}` | Remove an image from the profile. |

Sample requests are included in [`MediaVault.API/MediaVault.API.http`](MediaVault.API/MediaVault.API.http); update the variables at the top of the file to match your environment and execute with an HTTP client (e.g., Rider, VS Code REST Client).

## Validation Rules
- Maximum 10 images per profile (configurable).
- Maximum image size defaults to 3 MB (configurable).
- Allowed MIME types: `image/jpeg`, `image/png`, `image/webp` (configurable).
- Duplicate images (detected by SHA-256 hash) are rejected.

## Troubleshooting
- **Package restore timeouts:** Ensure outbound access to NuGet (`https://api.nuget.org`). Run `dotnet restore` again once network connectivity is available.
- **SQL connection failures:** Verify the connection string, credentials, and that SQL Server allows TCP connections.

