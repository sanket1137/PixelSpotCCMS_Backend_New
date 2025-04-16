using Microsoft.EntityFrameworkCore;
using PixelSpot.Domain.Entities;
using PixelSpot.Infrastructure.Data.Configurations;

namespace PixelSpot.Infrastructure.Data.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<SubUser> SubUsers { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Screen> Screens { get; set; }
    public DbSet<ScreenPricing> ScreenPricings { get; set; }
    public DbSet<ScreenAvailability> ScreenAvailabilities { get; set; }
    public DbSet<ScreenMetrics> ScreenMetrics { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Creative> Creatives { get; set; }
    public DbSet<ScreenBooking> ScreenBookings { get; set; }
    public DbSet<CampaignRequest> CampaignRequests { get; set; }
    public DbSet<WaitlistEntry> WaitlistEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SubUserConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionConfiguration());
        modelBuilder.ApplyConfiguration(new ScreenConfiguration());
        modelBuilder.ApplyConfiguration(new ScreenPricingConfiguration());
        modelBuilder.ApplyConfiguration(new ScreenAvailabilityConfiguration());
        modelBuilder.ApplyConfiguration(new ScreenMetricsConfiguration());
        modelBuilder.ApplyConfiguration(new CampaignConfiguration());
        modelBuilder.ApplyConfiguration(new CreativeConfiguration());
        modelBuilder.ApplyConfiguration(new ScreenBookingConfiguration());
        modelBuilder.ApplyConfiguration(new CampaignRequestConfiguration());
        modelBuilder.ApplyConfiguration(new WaitlistEntryConfiguration());
    }
}
