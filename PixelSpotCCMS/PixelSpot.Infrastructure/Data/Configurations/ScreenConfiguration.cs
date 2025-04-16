using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PixelSpot.Domain.Entities;
using PixelSpot.Domain.ValueObjects;

namespace PixelSpot.Infrastructure.Data.Configurations;

public class ScreenConfiguration : IEntityTypeConfiguration<Screen>
{
    public void Configure(EntityTypeBuilder<Screen> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Address)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.ImageUrl)
            .HasMaxLength(500);

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.Property(s => s.IsVerified)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.CreatedBy)
            .HasMaxLength(100);

        builder.Property(s => s.LastModifiedBy)
            .HasMaxLength(100);

        // Location as owned entity
        builder.OwnsOne(s => s.Location, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("Latitude");

            location.Property(l => l.Longitude)
                .HasColumnName("Longitude");
        });

        // Size as owned entity
        builder.OwnsOne(s => s.Size, size =>
        {
            size.Property(sz => sz.Width)
                .HasColumnName("Width");

            size.Property(sz => sz.Height)
                .HasColumnName("Height");

            size.Property(sz => sz.Unit)
                .HasMaxLength(20)
                .HasColumnName("Unit");
        });

        // Relationships
        builder.HasOne(s => s.Owner)
            .WithMany(u => u.Screens)
            .HasForeignKey(s => s.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Pricing)
            .WithOne(p => p.Screen)
            .HasForeignKey<ScreenPricing>(p => p.ScreenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Availabilities)
            .WithOne(a => a.Screen)
            .HasForeignKey(a => a.ScreenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Metrics)
            .WithOne(m => m.Screen)
            .HasForeignKey(m => m.ScreenId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Bookings)
            .WithOne(b => b.Screen)
            .HasForeignKey(b => b.ScreenId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ScreenPricingConfiguration : IEntityTypeConfiguration<ScreenPricing>
{
    public void Configure(EntityTypeBuilder<ScreenPricing> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.HourlyRate)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.DailyRate)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.WeeklyRate)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.MonthlyRate)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.MinimumBookingFee)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}

public class ScreenAvailabilityConfiguration : IEntityTypeConfiguration<ScreenAvailability>
{
    public void Configure(EntityTypeBuilder<ScreenAvailability> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.DayOfWeek)
            .IsRequired();

        builder.Property(a => a.StartTime)
            .IsRequired();

        builder.Property(a => a.EndTime)
            .IsRequired();
    }
}

public class ScreenMetricsConfiguration : IEntityTypeConfiguration<ScreenMetrics>
{
    public void Configure(EntityTypeBuilder<ScreenMetrics> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Date)
            .IsRequired();

        builder.Property(m => m.EstimatedViews)
            .IsRequired();

        builder.Property(m => m.EstimatedImpressions)
            .IsRequired();

        builder.Property(m => m.EstimatedEngagement)
            .IsRequired();

        builder.Property(m => m.AudienceDemographics)
            .HasMaxLength(500);
    }
}
