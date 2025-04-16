namespace PixelSpot.Application.DTOs;

public class ScreenDto
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public GeoCoordinateDto Location { get; set; } = new GeoCoordinateDto();
    public ScreenSizeDto Size { get; set; } = new ScreenSizeDto();
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public ScreenPricingDto? Pricing { get; set; }
    public List<ScreenAvailabilityDto> Availabilities { get; set; } = new List<ScreenAvailabilityDto>();
}

public class GeoCoordinateDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class ScreenSizeDto
{
    public double Width { get; set; }
    public double Height { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class CreateScreenDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public GeoCoordinateDto Location { get; set; } = new GeoCoordinateDto();
    public ScreenSizeDto Size { get; set; } = new ScreenSizeDto();
    public string ImageUrl { get; set; } = string.Empty;
    public ScreenPricingDto Pricing { get; set; } = new ScreenPricingDto();
    public List<ScreenAvailabilityDto> Availabilities { get; set; } = new List<ScreenAvailabilityDto>();
}

public class UpdateScreenDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public GeoCoordinateDto? Location { get; set; }
    public ScreenSizeDto? Size { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsActive { get; set; }
}

public class ScreenPricingDto
{
    public Guid Id { get; set; }
    public Guid ScreenId { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal WeeklyRate { get; set; }
    public decimal MonthlyRate { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal MinimumBookingFee { get; set; }
}

public class UpdateScreenPricingDto
{
    public decimal HourlyRate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal WeeklyRate { get; set; }
    public decimal MonthlyRate { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal MinimumBookingFee { get; set; }
}

public class ScreenAvailabilityDto
{
    public Guid Id { get; set; }
    public Guid ScreenId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public class CreateScreenAvailabilityDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public class ScreenMetricsDto
{
    public Guid Id { get; set; }
    public Guid ScreenId { get; set; }
    public DateTime Date { get; set; }
    public int EstimatedViews { get; set; }
    public int EstimatedImpressions { get; set; }
    public int EstimatedEngagement { get; set; }
    public string AudienceDemographics { get; set; } = string.Empty;
}

public class CreateScreenMetricsDto
{
    public DateTime Date { get; set; }
    public int EstimatedViews { get; set; }
    public int EstimatedImpressions { get; set; }
    public int EstimatedEngagement { get; set; }
    public string AudienceDemographics { get; set; } = string.Empty;
}

public class ScreenSearchDto
{
    public string? Keyword { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusInKm { get; set; }
    public string? Type { get; set; }
    public bool? IsVerified { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
