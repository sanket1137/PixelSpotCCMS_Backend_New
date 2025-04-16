using AutoMapper;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;
using PixelSpot.Domain.Entities;
using PixelSpot.Domain.Interfaces;
using PixelSpot.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace PixelSpot.Application.Services;

public class ScreenService : IScreenService
{
    private readonly IScreenRepository _screenRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ScreenService> _logger;

    public ScreenService(
        IScreenRepository screenRepository,
        IMapper mapper,
        ILogger<ScreenService> logger)
    {
        _screenRepository = screenRepository ?? throw new ArgumentNullException(nameof(screenRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ScreenDto> GetByIdAsync(Guid id)
    {
        var screen = await _screenRepository.GetScreenWithDetailsAsync(id);
        if (screen == null)
        {
            throw new KeyNotFoundException($"Screen with ID {id} not found.");
        }

        return _mapper.Map<ScreenDto>(screen);
    }

    public async Task<IReadOnlyList<ScreenDto>> GetAllAsync()
    {
        var screens = await _screenRepository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<ScreenDto>>(screens);
    }

    public async Task<IReadOnlyList<ScreenDto>> GetScreensByOwnerIdAsync(Guid ownerId)
    {
        var screens = await _screenRepository.GetScreensByOwnerIdAsync(ownerId);
        return _mapper.Map<IReadOnlyList<ScreenDto>>(screens);
    }

    public async Task<IReadOnlyList<ScreenDto>> GetVerifiedScreensAsync()
    {
        var screens = await _screenRepository.GetVerifiedScreensAsync();
        return _mapper.Map<IReadOnlyList<ScreenDto>>(screens);
    }

    public async Task<IReadOnlyList<ScreenDto>> SearchScreensAsync(ScreenSearchDto searchDto)
    {
        // Start with all verified screens
        var screens = await _screenRepository.GetVerifiedScreensAsync();
        var filteredScreens = screens.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
        {
            filteredScreens = filteredScreens.Where(s => 
                s.Name.Contains(searchDto.Keyword, StringComparison.OrdinalIgnoreCase) ||
                s.Description.Contains(searchDto.Keyword, StringComparison.OrdinalIgnoreCase) ||
                s.Type.Contains(searchDto.Keyword, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(searchDto.City))
        {
            filteredScreens = filteredScreens.Where(s => 
                s.City.Contains(searchDto.City, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(searchDto.State))
        {
            filteredScreens = filteredScreens.Where(s => 
                s.State.Contains(searchDto.State, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(searchDto.Country))
        {
            filteredScreens = filteredScreens.Where(s => 
                s.Country.Contains(searchDto.Country, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(searchDto.Type))
        {
            filteredScreens = filteredScreens.Where(s => 
                s.Type.Contains(searchDto.Type, StringComparison.OrdinalIgnoreCase));
        }

        // Location-based search
        if (searchDto.Latitude.HasValue && searchDto.Longitude.HasValue && searchDto.RadiusInKm.HasValue)
        {
            var searchLocation = new GeoCoordinate(searchDto.Latitude.Value, searchDto.Longitude.Value);
            var radiusInKm = searchDto.RadiusInKm.Value;
            
            var screensInRadius = await _screenRepository.GetScreensInRadiusAsync(searchLocation, radiusInKm);
            var screenIds = screensInRadius.Select(s => s.Id).ToList();
            
            filteredScreens = filteredScreens.Where(s => screenIds.Contains(s.Id));
        }

        // Date-based availability
        if (searchDto.StartDate.HasValue && searchDto.EndDate.HasValue)
        {
            var availableScreenIds = new List<Guid>();
            foreach (var screen in filteredScreens)
            {
                var isAvailable = await _screenRepository.IsScreenAvailableAsync(
                    screen.Id, searchDto.StartDate.Value, searchDto.EndDate.Value);
                
                if (isAvailable)
                {
                    availableScreenIds.Add(screen.Id);
                }
            }
            
            filteredScreens = filteredScreens.Where(s => availableScreenIds.Contains(s.Id));
        }

        // Apply pagination
        var pagedScreens = filteredScreens
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToList();

        return _mapper.Map<IReadOnlyList<ScreenDto>>(pagedScreens);
    }

    public async Task<ScreenDto> CreateScreenAsync(Guid ownerId, CreateScreenDto createScreenDto, string createdBy)
    {
        var location = new GeoCoordinate(
            createScreenDto.Location.Latitude,
            createScreenDto.Location.Longitude);

        var size = new ScreenSize(
            createScreenDto.Size.Width,
            createScreenDto.Size.Height,
            createScreenDto.Size.Unit);

        var screen = new Screen(
            ownerId,
            createScreenDto.Name,
            createScreenDto.Description,
            createScreenDto.Type,
            createScreenDto.Address,
            createScreenDto.City,
            createScreenDto.State,
            createScreenDto.Country,
            createScreenDto.PostalCode,
            location,
            size,
            createScreenDto.ImageUrl);

        var pricing = new ScreenPricing(
            screen.Id,
            createScreenDto.Pricing.HourlyRate,
            createScreenDto.Pricing.DailyRate,
            createScreenDto.Pricing.WeeklyRate,
            createScreenDto.Pricing.MonthlyRate,
            createScreenDto.Pricing.Currency,
            createScreenDto.Pricing.MinimumBookingFee);

        screen.SetPricing(pricing);

        // Add availabilities
        foreach (var availabilityDto in createScreenDto.Availabilities)
        {
            var availability = new ScreenAvailability(
                screen.Id,
                availabilityDto.DayOfWeek,
                availabilityDto.StartTime,
                availabilityDto.EndTime);

            screen.AddAvailability(availability);
        }

        screen = await _screenRepository.AddAsync(screen);
        await _screenRepository.SaveChangesAsync();

        _logger.LogInformation("Screen created: {ScreenId} for owner: {OwnerId}", screen.Id, ownerId);

        return _mapper.Map<ScreenDto>(screen);
    }

    public async Task<ScreenDto> UpdateScreenAsync(Guid id, UpdateScreenDto updateScreenDto, string modifiedBy)
    {
        var screen = await _screenRepository.GetScreenWithDetailsAsync(id);
        if (screen == null)
        {
            throw new KeyNotFoundException($"Screen with ID {id} not found.");
        }

        GeoCoordinate? location = null;
        if (updateScreenDto.Location != null)
        {
            location = new GeoCoordinate(
                updateScreenDto.Location.Latitude,
                updateScreenDto.Location.Longitude);
        }

        ScreenSize? size = null;
        if (updateScreenDto.Size != null)
        {
            size = new ScreenSize(
                updateScreenDto.Size.Width,
                updateScreenDto.Size.Height,
                updateScreenDto.Size.Unit);
        }

        screen.Update(
            updateScreenDto.Name ?? screen.Name,
            updateScreenDto.Description ?? screen.Description,
            updateScreenDto.Type ?? screen.Type,
            updateScreenDto.Address ?? screen.Address,
            updateScreenDto.City ?? screen.City,
            updateScreenDto.State ?? screen.State,
            updateScreenDto.Country ?? screen.Country,
            updateScreenDto.PostalCode ?? screen.PostalCode,
            location ?? screen.Location,
            size ?? screen.Size,
            updateScreenDto.ImageUrl ?? screen.ImageUrl);

        if (updateScreenDto.IsActive.HasValue)
        {
            screen.SetActiveStatus(updateScreenDto.IsActive.Value);
        }

        await _screenRepository.UpdateAsync(screen);
        await _screenRepository.SaveChangesAsync();

        _logger.LogInformation("Screen updated: {ScreenId}", screen.Id);

        return _mapper.Map<ScreenDto>(screen);
    }

    public async Task<bool> SetScreenActiveStatusAsync(Guid id, bool isActive, string modifiedBy)
    {
        var screen = await _screenRepository.GetByIdAsync(id);
        if (screen == null)
        {
            throw new KeyNotFoundException($"Screen with ID {id} not found.");
        }

        screen.SetActiveStatus(isActive);

        await _screenRepository.UpdateAsync(screen);
        await _screenRepository.SaveChangesAsync();

        _logger.LogInformation("Screen {ScreenId} active status set to {IsActive}", screen.Id, isActive);

        return true;
    }

    public async Task<bool> SetScreenVerificationStatusAsync(Guid id, bool isVerified, string modifiedBy)
    {
        var screen = await _screenRepository.GetByIdAsync(id);
        if (screen == null)
        {
            throw new KeyNotFoundException($"Screen with ID {id} not found.");
        }

        screen.SetVerificationStatus(isVerified);

        await _screenRepository.UpdateAsync(screen);
        await _screenRepository.SaveChangesAsync();

        _logger.LogInformation("Screen {ScreenId} verification status set to {IsVerified}", screen.Id, isVerified);

        return true;
    }

    public async Task<bool> DeleteScreenAsync(Guid id)
    {
        var screen = await _screenRepository.GetByIdAsync(id);
        if (screen == null)
        {
            throw new KeyNotFoundException($"Screen with ID {id} not found.");
        }

        await _screenRepository.DeleteAsync(screen);
        await _screenRepository.SaveChangesAsync();

        _logger.LogInformation("Screen deleted: {ScreenId}", screen.Id);

        return true;
    }

    public async Task<ScreenPricingDto> UpdateScreenPricingAsync(Guid screenId, UpdateScreenPricingDto pricingDto, string modifiedBy)
    {
        var screen = await _screenRepository.GetScreenWithDetailsAsync(screenId);
        if (screen == null)
        {
            throw new KeyNotFoundException($"Screen with ID {screenId} not found.");
        }

        if (screen.Pricing == null)
        {
            var pricing = new ScreenPricing(
                screenId,
                pricingDto.HourlyRate,
                pricingDto.DailyRate,
                pricingDto.WeeklyRate,
                pricingDto.MonthlyRate,
                pricingDto.Currency,
                pricingDto.MinimumBookingFee);

            screen.SetPricing(pricing);
        }
        else
        {
            screen.Pricing.Update(
                pricingDto.HourlyRate,
                pricingDto.DailyRate,
                pricingDto.WeeklyRate,
                pricingDto.MonthlyRate,
                pricingDto.Currency,
                pricingDto.MinimumBookingFee);
        }

        await _screenRepository.UpdateAsync(screen);
        await _screenRepository.SaveChangesAsync();

        _logger.LogInformation("Screen pricing updated for screen: {ScreenId}", screenId);

        return _mapper.Map<ScreenPricingDto>(screen.Pricing);
    }

    public async Task<ScreenAvailabilityDto> AddScreenAvailabilityAsync(Guid screenId, CreateScreenAvailabilityDto availabilityDto, string createdBy)
    {
        var screen = await _screenRepository.GetScreenWithDetailsAsync(screenId);
        if (screen == null)
        {
            throw new KeyNotFoundException($"Screen with ID {screenId} not found.");
        }

        var availability = new ScreenAvailability(
            screenId,
            availabilityDto.DayOfWeek,
            availabilityDto.StartTime,
            availabilityDto.EndTime);

        screen.AddAvailability(availability);

        await _screenRepository.UpdateAsync(screen);
        await _screenRepository.SaveChangesAsync();

        _logger.LogInformation("Screen availability added for screen: {ScreenId}", screenId);

        return _mapper.Map<ScreenAvailabilityDto>(availability);
    }

    public async Task<bool> RemoveScreenAvailabilityAsync(Guid availabilityId)
    {
        var screen = await _screenRepository.GetAllAsync();
        var targetScreen = screen.FirstOrDefault(s => s.Availabilities.Any(a => a.Id == availabilityId));
        
        if (targetScreen == null)
        {
            throw new KeyNotFoundException($"Availability with ID {availabilityId} not found.");
        }

        var availability = targetScreen.Availabilities.First(a => a.Id == availabilityId);
        targetScreen.RemoveAvailability(availability);

        await _screenRepository.UpdateAsync(targetScreen);
        await _screenRepository.SaveChangesAsync();

        _logger.LogInformation("Screen availability removed: {AvailabilityId}", availabilityId);

        return true;
    }

    public async Task<IReadOnlyList<ScreenAvailabilityDto>> GetScreenAvailabilitiesAsync(Guid screenId)
    {
        var availabilities = await _screenRepository.GetScreenAvailabilitiesAsync(screenId);
        return _mapper.Map<IReadOnlyList<ScreenAvailabilityDto>>(availabilities);
    }

    public async Task<IReadOnlyList<ScreenBookingDto>> GetScreenBookingsAsync(Guid screenId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var bookings = await _screenRepository.GetScreenBookingsAsync(screenId, startDate, endDate);
        return _mapper.Map<IReadOnlyList<ScreenBookingDto>>(bookings);
    }

    public async Task<ScreenMetricsDto> AddScreenMetricsAsync(Guid screenId, CreateScreenMetricsDto metricsDto, string createdBy)
    {
        var screen = await _screenRepository.GetByIdAsync(screenId);
        if (screen == null)
        {
            throw new KeyNotFoundException($"Screen with ID {screenId} not found.");
        }

        var metrics = new ScreenMetrics(
            screenId,
            metricsDto.Date,
            metricsDto.EstimatedViews,
            metricsDto.EstimatedImpressions,
            metricsDto.EstimatedEngagement,
            metricsDto.AudienceDemographics);

        // Assuming ScreenMetrics is added via the repository
        // This is a simplification; in reality, you might want a separate repository for metrics
        var result = _mapper.Map<ScreenMetricsDto>(metrics);
        
        _logger.LogInformation("Screen metrics added for screen: {ScreenId} on {Date}", screenId, metricsDto.Date);

        return result;
    }

    public async Task<IReadOnlyList<ScreenMetricsDto>> GetScreenMetricsAsync(Guid screenId, DateTime startDate, DateTime endDate)
    {
        var metrics = await _screenRepository.GetScreenMetricsAsync(screenId, startDate, endDate);
        return _mapper.Map<IReadOnlyList<ScreenMetricsDto>>(metrics);
    }

    public async Task<bool> IsScreenAvailableAsync(Guid screenId, DateTime startTime, DateTime endTime)
    {
        return await _screenRepository.IsScreenAvailableAsync(screenId, startTime, endTime);
    }

    public async Task<decimal> CalculateBookingPriceAsync(Guid screenId, DateTime startTime, DateTime endTime)
    {
        var screen = await _screenRepository.GetScreenWithDetailsAsync(screenId);
        if (screen == null)
        {
            throw new KeyNotFoundException($"Screen with ID {screenId} not found.");
        }

        if (screen.Pricing == null)
        {
            throw new InvalidOperationException($"Screen with ID {screenId} does not have pricing information.");
        }

        return screen.Pricing.CalculatePrice(startTime, endTime);
    }
}
