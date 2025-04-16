using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelSpot.Domain.Entities;
using PixelSpot.Domain.ValueObjects;

namespace PixelSpot.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.CompanyName)
            .HasMaxLength(100);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(u => u.IsVerified)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.CreatedBy)
            .HasMaxLength(100);

        builder.Property(u => u.LastModifiedBy)
            .HasMaxLength(100);

        // Bank details as owned entity
        builder.OwnsOne(u => u.BankDetails, bankDetails =>
        {
            bankDetails.Property(b => b.AccountHolderName)
                .HasMaxLength(100)
                .HasColumnName("BankAccountHolderName");

            bankDetails.Property(b => b.BankName)
                .HasMaxLength(100)
                .HasColumnName("BankName");

            bankDetails.Property(b => b.AccountNumber)
                .HasMaxLength(50)
                .HasColumnName("BankAccountNumber");

            bankDetails.Property(b => b.RoutingNumber)
                .HasMaxLength(50)
                .HasColumnName("BankRoutingNumber");
        });

        // Unique constraint for email
        builder.HasIndex(u => u.Email)
            .IsUnique();

        // Relationships
        builder.HasMany(u => u.SubUsers)
            .WithOne(su => su.User)
            .HasForeignKey(su => su.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Screens)
            .WithOne(s => s.Owner)
            .HasForeignKey(s => s.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Campaigns)
            .WithOne(c => c.Advertiser)
            .HasForeignKey(c => c.AdvertiserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SubUserConfiguration : IEntityTypeConfiguration<SubUser>
{
    public void Configure(EntityTypeBuilder<SubUser> builder)
    {
        builder.HasKey(su => su.Id);

        builder.Property(su => su.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(su => su.PasswordHash)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(su => su.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(su => su.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(su => su.IsActive)
            .IsRequired();

        builder.Property(su => su.CreatedAt)
            .IsRequired();

        builder.Property(su => su.CreatedBy)
            .HasMaxLength(100);

        builder.Property(su => su.LastModifiedBy)
            .HasMaxLength(100);

        // Unique constraint for email
        builder.HasIndex(su => su.Email)
            .IsUnique();

        // Many-to-many relationship with Permission
        builder.HasMany(su => su.Permissions)
            .WithMany(p => p.SubUsers)
            .UsingEntity(j => j.ToTable("SubUserPermissions"));
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Description)
            .HasMaxLength(200);

        // Unique constraint for name
        builder.HasIndex(p => p.Name)
            .IsUnique();
    }
}
