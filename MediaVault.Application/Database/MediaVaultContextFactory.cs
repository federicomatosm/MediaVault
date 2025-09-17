using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MediaVault.Application.Database;

public class MediaVaultContextFactory: IDesignTimeDbContextFactory<MediaVaultContext>
{
    public MediaVaultContext CreateDbContext(string[] args)
    {
        var app = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
           
            .AddJsonFile(Path.Combine("..", "MediaVault.API", "appsettings.json"), optional: true)
            .AddJsonFile(Path.Combine("..", "MediaVault.API", "appsettings.Development.json"), optional: true)
            .AddEnvironmentVariables()
            .Build();

        var conn = app.GetConnectionString("MediaVaultDb")
                   ?? System.Environment.GetEnvironmentVariable("MediaVault_ConnectionString");

        if (string.IsNullOrWhiteSpace(conn))
            throw new InvalidOperationException(
                "No connection string found. Define 'ConnectionStrings:MediaVaultDb' in appsettings or set env var 'MediaVault_ConnectionString'.");

        var options = new DbContextOptionsBuilder<MediaVaultContext>()
            .UseSqlServer(conn)
            .Options;

        return new MediaVaultContext(options);
    }
}