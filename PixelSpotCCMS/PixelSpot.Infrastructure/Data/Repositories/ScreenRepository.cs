using Microsoft.EntityFrameworkCore;
using PixelSpot.Domain.Entities;
using PixelSpot.Domain.Interfaces;
using PixelSpot.Domain.ValueObjects;
using PixelSpot.Infrastructure.Data.Contexts;

namespace PixelSpot.Infrastructure.Data.Repositories;

public class ScreenRepository : BaseRepository<Screen>, IScreenRepository
{
    public ScreenRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Screen>> GetScreensByOwnerIdAsync(Guid ownerId)
    {
        return await _dbContext.Screens
            .Where(s => s.OwnerId == ownerId)
            .Include(s => s.Pricing)
            .Include(s => s.Availabilities)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Screen>> GetVerifiedScreensAsync()
    {
        return await _dbContext.Screens
            .Where(s => s.IsVerified && s.IsActive)
            .Include(s => s.Pricing)
            .Include(s => s.Availabilities)
            .Include(s => s.Owner)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Screen>> GetScreensInRadiusAsync(GeoCoordinate location, double radiusInKm)
    {
        // For demonstration purposes, fetch all screens and filter them in memory
        // In a production environment, this should be done with a spatial query
        var allScreens = await _dbContext.Screens
            .Where(s => s.IsVerified && s.IsActive)
            .Include(s => s.Pricing)
            .Include(s => s.Availabilities)
            .ToListAsync();

        return allScreens
            .Where(s => s.Location.CalculateDistance(location) <= radiusInKm)
            .ToList();
    }

    public async Task<Screen?> GetScreenWithDetailsAsync(Guid id)
    {
        return await _dbContext.Screens
            .Include(s => s.Owner)
            .Include(s => s.Pricing)
            .Include(s => s.Availabilities)
            .Include(s => s.Metrics)
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IReadOnlyList<ScreenAvailability>> GetScreenAvailabilitiesAsync(Guid screenId)
    {
        return await _dbContext.ScreenAvailabilities
            .Where(a => a.ScreenId == screenId)
            .ToListAsync();
    }

    public async Task<bool> IsScreenAvailableAsync(Guid screenId, DateTime startTime, DateTime endTime)
    {
        var screen = await _dbContext.Screens
            .Include(s => s.Availabilities)
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == screenId);

        if (screen == null || !screen.IsActive || !screen.IsVerified)
        {
            return false;
        }

        // Check if the screen is available during the requested time slot based on its availabilities
        if (!screen.IsAvailable(startTime, endTime))
        {
            return false;
        }

        // Check if there are any overlapping bookings
        var overlappingBookings = screen.Bookings
            .Where(b => b.Status != "Cancelled")
            .Any(b => b.IsOverlapping(startTime, endTime));

        return !overlappingBookings;
    }

    public async Task<IReadOnlyList<ScreenBooking>> GetScreenBookingsAsync(Guid screenId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _dbContext.ScreenBookings
            .Where(b => b.ScreenId == screenId);

        if (startDate.HasValue)
        {
            query = query.Where(b => b.EndTime >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(b => b.StartTime <= endDate.Value);
        }

        return await query
            .Include(b => b.Campaign)
            .Include(b => b.Creative)
            .OrderBy(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ScreenMetrics>> GetScreenMetricsAsync(Guid screenId, DateTime startDate, DateTime endDate)
    {
        return await _dbContext.ScreenMetrics
            .Where(m => m.ScreenId == screenId && m.Date >= startDate && m.Date <= endDate)
            .OrderBy(m => m.Date)
            .ToListAsync();
    }
}
