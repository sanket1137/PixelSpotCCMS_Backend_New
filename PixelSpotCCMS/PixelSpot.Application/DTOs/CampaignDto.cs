namespace PixelSpot.Application.DTOs;

public class CampaignDto
{
    public Guid Id { get; set; }
    public Guid AdvertiserId { get; set; }
    public string AdvertiserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Budget { get; set; }
    public string Status { get; set; } = string.Empty;
    public string TargetAudience { get; set; } = string.Empty;
    public string TargetLocations { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public decimal Spent { get; set; }
    public decimal Remaining { get; set; }
    public List<CreativeDto> Creatives { get; set; } = new List<CreativeDto>();
    public List<ScreenBookingDto> Bookings { get; set; } = new List<ScreenBookingDto>();
}

public class CreateCampaignDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Budget { get; set; }
    public string TargetAudience { get; set; } = string.Empty;
    public string TargetLocations { get; set; } = string.Empty;
}

public class UpdateCampaignDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public string? TargetAudience { get; set; }
    public string? TargetLocations { get; set; }
    public string? Status { get; set; }
}

public class CreativeDto
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ContentUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public bool IsApproved { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

public class CreateCreativeDto
{
    public Guid CampaignId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ContentUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
}

public class UpdateCreativeDto
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? ContentUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? DurationSeconds { get; set; }
}

public class ApproveRejectCreativeDto
{
    public bool IsApproved { get; set; }
    public string? RejectionReason { get; set; }
}

public class ScreenBookingDto
{
    public Guid Id { get; set; }
    public Guid ScreenId { get; set; }
    public string ScreenName { get; set; } = string.Empty;
    public Guid CampaignId { get; set; }
    public Guid CreativeId { get; set; }
    public string CreativeName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentReference { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

public class CreateScreenBookingDto
{
    public Guid ScreenId { get; set; }
    public Guid CampaignId { get; set; }
    public Guid CreativeId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class UpdateScreenBookingStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class UpdateScreenBookingPaymentDto
{
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentReference { get; set; } = string.Empty;
}

public class CampaignRequestDto
{
    public Guid Id { get; set; }
    public Guid AdvertiserId { get; set; }
    public string AdvertiserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Budget { get; set; }
    public string TargetAudience { get; set; } = string.Empty;
    public string TargetLocations { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

public class CreateCampaignRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Budget { get; set; }
    public string TargetAudience { get; set; } = string.Empty;
    public string TargetLocations { get; set; } = string.Empty;
}

public class UpdateCampaignRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public string? TargetAudience { get; set; }
    public string? TargetLocations { get; set; }
}

public class ApproveCampaignRequestDto
{
    public bool IsApproved { get; set; }
    public string? RejectionReason { get; set; }
}

public class WaitlistEntryDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? InvitationSentDate { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public GeoCoordinateDto? Location { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

public class CreateWaitlistEntryDto
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public GeoCoordinateDto? Location { get; set; }
}

public class UpdateWaitlistEntryDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CompanyName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? UserType { get; set; }
    public GeoCoordinateDto? Location { get; set; }
}

public class UpdateWaitlistEntryStatusDto
{
    public string Status { get; set; } = string.Empty;
}
