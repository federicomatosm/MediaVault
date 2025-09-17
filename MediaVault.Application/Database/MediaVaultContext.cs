using MediaVault.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaVault.Application.Database;

public sealed class MediaVaultContext(DbContextOptions<MediaVaultContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ProfileImage> ProfileImages {
        get;
        set;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var customerEntity = modelBuilder.Entity<Customer>();
        customerEntity.ToTable("Customers", "dbo");
        customerEntity.HasKey(x=>x.Id);
        customerEntity.Property(x=>x.Name).HasColumnType("varchar(255)").IsRequired();
        customerEntity.Property(x=>x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()");

        var profileImageEntity = modelBuilder.Entity<ProfileImage>();
        profileImageEntity.ToTable("ProfileImages", "dbo");
        profileImageEntity.HasKey(x => x.Id);
        profileImageEntity.HasOne(x=>x.Owner).WithMany(i=>i.Images).HasForeignKey(x=>x.OwnerId);
        profileImageEntity.Property(x => x.OwnerType).HasConversion<byte>();
        profileImageEntity.Property(x => x.Base64Data).HasColumnType("varchar(max)").IsRequired();
        profileImageEntity.Property(x => x.MimeType).HasMaxLength(64).IsRequired();
        profileImageEntity.Property(x => x.OriginalFileName).HasMaxLength(255);
        profileImageEntity.Property(x => x.ContentHashSha256).HasColumnType("varbinary(32)").IsRequired();
        profileImageEntity.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()");
        profileImageEntity.HasIndex(x => new { x.OwnerType, x.OwnerId, x.CreatedUtc }).HasDatabaseName("IX_ProfileImages_Owner");
        profileImageEntity.HasIndex(x => new { x.OwnerType, x.OwnerId, x.ContentHashSha256 }).IsUnique()
            .HasDatabaseName("UX_ProfileImages_Owner_Hash");
    }
}