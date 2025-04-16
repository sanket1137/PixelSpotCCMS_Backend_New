using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelSpot.Domain.Entities;

namespace PixelSpot.Infrastructure.Data.Configurations;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.StartDate)
            .IsRequired();

        builder.Property(c => c.EndDate)
            .IsRequired();

        builder.Property(c => c.Budget)
            .HasPrecision(18, 2) // Corrected usage for decimals  
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.TargetAudience)
            .HasMaxLength(200);

        builder.Property(c => c.TargetLocations)
            .HasMaxLength(200);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100);

        builder.Property(c => c.LastModifiedBy)
            .HasMaxLength(100);

        // Relationships  
        builder.HasOne(c => c.Advertiser)
            .WithMany(u => u.Campaigns)
            .HasForeignKey(c => c.AdvertiserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Creatives)
            .WithOne(cr => cr.Campaign)
            .HasForeignKey(cr => cr.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Bookings)
            .WithOne(b => b.Campaign)
            .HasForeignKey(b => b.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class CreativeConfiguration : IEntityTypeConfiguration<Creative>
{
    public void Configure(EntityTypeBuilder<Creative> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Type)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.ContentUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(c => c.DurationSeconds)
            .IsRequired();

        builder.Property(c => c.IsApproved)
            .IsRequired();

        builder.Property(c => c.RejectionReason)
            .HasMaxLength(500);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasMaxLength(100);

        builder.Property(c => c.LastModifiedBy)
            .HasMaxLength(100);
    }
}

public class ScreenBookingConfiguration : IEntityTypeConfiguration<ScreenBooking>
{
    public void Configure(EntityTypeBuilder<ScreenBooking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.StartTime)
            .IsRequired();

        builder.Property(b => b.EndTime)
            .IsRequired();

        builder.Property(b => b.Price)
            .HasPrecision(18, 2) // Corrected usage for decimals  
            .IsRequired();

        builder.Property(b => b.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.PaymentStatus)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(b => b.PaymentReference)
            .HasMaxLength(100);

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.CreatedBy)
            .HasMaxLength(100);

        builder.Property(b => b.LastModifiedBy)
            .HasMaxLength(100);

        // Relationships  
        builder.HasOne(b => b.Screen)
            .WithMany(s => s.Bookings)
            .HasForeignKey(b => b.ScreenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Campaign)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Creative)
            .WithMany()
            .HasForeignKey(b => b.CreativeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CampaignRequestConfiguration : IEntityTypeConfiguration<CampaignRequest>
{
    public void Configure(EntityTypeBuilder<CampaignRequest> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.StartDate)
            .IsRequired();

        builder.Property(r => r.EndDate)
            .IsRequired();

        builder.Property(r => r.Budget)
            .HasPrecision(18, 2) // Corrected usage for decimals  
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(r => r.RejectionReason)
            .HasMaxLength(500);

        builder.Property(r => r.TargetAudience)
            .HasMaxLength(200);

        builder.Property(r => r.TargetLocations)
            .HasMaxLength(200);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.CreatedBy)
            .HasMaxLength(100);

        builder.Property(r => r.LastModifiedBy)
            .HasMaxLength(100);

        // Relationships  
        builder.HasOne(r => r.Advertiser)
            .WithMany()
            .HasForeignKey(r => r.AdvertiserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class WaitlistEntryConfiguration : IEntityTypeConfiguration<WaitlistEntry>
{
    public void Configure(EntityTypeBuilder<WaitlistEntry> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.CompanyName)
            .HasMaxLength(100);

        builder.Property(w => w.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(w => w.UserType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(w => w.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(w => w.CreatedAt)
            .IsRequired();

        builder.Property(w => w.CreatedBy)
            .HasMaxLength(100);

        builder.Property(w => w.LastModifiedBy)
            .HasMaxLength(100);

        // Location as owned entity
        builder.OwnsOne(w => w.Location, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("Latitude");

            location.Property(l => l.Longitude)
                .HasColumnName("Longitude");
        });

        // Unique constraint for email
        builder.HasIndex(w => w.Email)
            .IsUnique();
    }
}
