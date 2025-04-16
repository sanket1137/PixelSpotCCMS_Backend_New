using PixelSpot.Domain.Entities;
using PixelSpot.Domain.ValueObjects;

namespace PixelSpot.Domain.Interfaces;

public interface IScreenRepository : IRepository<Screen>
{
    Task<IReadOnlyList<Screen>> GetScreensByOwnerIdAsync(Guid ownerId);
    Task<IReadOnlyList<Screen>> GetVerifiedScreensAsync();
    Task<IReadOnlyList<Screen>> GetScreensInRadiusAsync(GeoCoordinate location, double radiusInKm);
    Task<Screen?> GetScreenWithDetailsAsync(Guid id);
    Task<IReadOnlyList<ScreenAvailability>> GetScreenAvailabilitiesAsync(Guid screenId);
    Task<bool> IsScreenAvailableAsync(Guid screenId, DateTime startTime, DateTime endTime);
    Task<IReadOnlyList<ScreenBooking>> GetScreenBookingsAsync(Guid screenId, DateTime? startDate = null, DateTime? endDate = null);
    Task<IReadOnlyList<ScreenMetrics>> GetScreenMetricsAsync(Guid screenId, DateTime startDate, DateTime endDate);
}
