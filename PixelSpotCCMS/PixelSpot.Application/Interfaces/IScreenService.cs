using PixelSpot.Application.DTOs;

namespace PixelSpot.Application.Interfaces;

public interface IScreenService
{
    Task<ScreenDto> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ScreenDto>> GetAllAsync();
    Task<IReadOnlyList<ScreenDto>> GetScreensByOwnerIdAsync(Guid ownerId);
    Task<IReadOnlyList<ScreenDto>> GetVerifiedScreensAsync();
    Task<IReadOnlyList<ScreenDto>> SearchScreensAsync(ScreenSearchDto searchDto);
    Task<ScreenDto> CreateScreenAsync(Guid ownerId, CreateScreenDto createScreenDto, string createdBy);
    Task<ScreenDto> UpdateScreenAsync(Guid id, UpdateScreenDto updateScreenDto, string modifiedBy);
    Task<bool> SetScreenActiveStatusAsync(Guid id, bool isActive, string modifiedBy);
    Task<bool> SetScreenVerificationStatusAsync(Guid id, bool isVerified, string modifiedBy);
    Task<bool> DeleteScreenAsync(Guid id);
    Task<ScreenPricingDto> UpdateScreenPricingAsync(Guid screenId, UpdateScreenPricingDto pricingDto, string modifiedBy);
    Task<ScreenAvailabilityDto> AddScreenAvailabilityAsync(Guid screenId, CreateScreenAvailabilityDto availabilityDto, string createdBy);
    Task<bool> RemoveScreenAvailabilityAsync(Guid availabilityId);
    Task<IReadOnlyList<ScreenAvailabilityDto>> GetScreenAvailabilitiesAsync(Guid screenId);
    Task<IReadOnlyList<ScreenBookingDto>> GetScreenBookingsAsync(Guid screenId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ScreenMetricsDto> AddScreenMetricsAsync(Guid screenId, CreateScreenMetricsDto metricsDto, string createdBy);
    Task<IReadOnlyList<ScreenMetricsDto>> GetScreenMetricsAsync(Guid screenId, DateTime startDate, DateTime endDate);
    Task<bool> IsScreenAvailableAsync(Guid screenId, DateTime startTime, DateTime endTime);
    Task<decimal> CalculateBookingPriceAsync(Guid screenId, DateTime startTime, DateTime endTime);
}
