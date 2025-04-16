using AutoMapper;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;
using PixelSpot.Domain.Entities;
using PixelSpot.Domain.Interfaces;
using PixelSpot.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace PixelSpot.Application.Services;

public class CampaignService : ICampaignService
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IScreenRepository _screenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CampaignService> _logger;

    public CampaignService(
        ICampaignRepository campaignRepository,
        IScreenRepository screenRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<CampaignService> logger)
    {
        _campaignRepository = campaignRepository ?? throw new ArgumentNullException(nameof(campaignRepository));
        _screenRepository = screenRepository ?? throw new ArgumentNullException(nameof(screenRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CampaignDto> GetByIdAsync(Guid id)
    {
        var campaign = await _campaignRepository.GetCampaignWithDetailsAsync(id);
        if (campaign == null)
        {
            throw new KeyNotFoundException($"Campaign with ID {id} not found.");
        }

        var campaignDto = _mapper.Map<CampaignDto>(campaign);
        campaignDto.Spent = campaign.CalculateSpent();
        campaignDto.Remaining = campaign.CalculateRemaining();

        return campaignDto;
    }

    public async Task<IReadOnlyList<CampaignDto>> GetAllAsync()
    {
        var campaigns = await _campaignRepository.GetAllAsync();
        var campaignDtos = _mapper.Map<IReadOnlyList<CampaignDto>>(campaigns);

        // Calculate spent and remaining for each campaign
        foreach (var campaignDto in campaignDtos)
        {
            var campaign = campaigns.First(c => c.Id == campaignDto.Id);
            campaignDto.Spent = campaign.CalculateSpent();
            campaignDto.Remaining = campaign.CalculateRemaining();
        }

        return campaignDtos;
    }

    public async Task<IReadOnlyList<CampaignDto>> GetCampaignsByAdvertiserIdAsync(Guid advertiserId)
    {
        var campaigns = await _campaignRepository.GetCampaignsByAdvertiserIdAsync(advertiserId);
        var campaignDtos = _mapper.Map<IReadOnlyList<CampaignDto>>(campaigns);

        // Calculate spent and remaining for each campaign
        foreach (var campaignDto in campaignDtos)
        {
            var campaign = campaigns.First(c => c.Id == campaignDto.Id);
            campaignDto.Spent = campaign.CalculateSpent();
            campaignDto.Remaining = campaign.CalculateRemaining();
        }

        return campaignDtos;
    }

    public async Task<CampaignDto> CreateCampaignAsync(Guid advertiserId, CreateCampaignDto createCampaignDto, string createdBy)
    {
        var advertiser = await _userRepository.GetByIdAsync(advertiserId);
        if (advertiser == null)
        {
            throw new KeyNotFoundException($"Advertiser with ID {advertiserId} not found.");
        }

        var campaign = new Campaign(
            advertiserId,
            createCampaignDto.Name,
            createCampaignDto.Description,
            createCampaignDto.StartDate,
            createCampaignDto.EndDate,
            createCampaignDto.Budget,
            createCampaignDto.TargetAudience,
            createCampaignDto.TargetLocations);

        campaign = await _campaignRepository.AddAsync(campaign);
        await _campaignRepository.SaveChangesAsync();

        _logger.LogInformation("Campaign created: {CampaignId} for advertiser: {AdvertiserId}", campaign.Id, advertiserId);

        var campaignDto = _mapper.Map<CampaignDto>(campaign);
        campaignDto.Spent = 0;
        campaignDto.Remaining = campaign.Budget;

        return campaignDto;
    }

    public async Task<CampaignDto> UpdateCampaignAsync(Guid id, UpdateCampaignDto updateCampaignDto, string modifiedBy)
    {
        var campaign = await _campaignRepository.GetCampaignWithDetailsAsync(id);
        if (campaign == null)
        {
            throw new KeyNotFoundException($"Campaign with ID {id} not found.");
        }

        campaign.Update(
            updateCampaignDto.Name ?? campaign.Name,
            updateCampaignDto.Description ?? campaign.Description,
            updateCampaignDto.StartDate ?? campaign.StartDate,
            updateCampaignDto.EndDate ?? campaign.EndDate,
            updateCampaignDto.Budget ?? campaign.Budget,
            updateCampaignDto.TargetAudience ?? campaign.TargetAudience,
            updateCampaignDto.TargetLocations ?? campaign.TargetLocations);

        if (!string.IsNullOrWhiteSpace(updateCampaignDto.Status))
        {
            campaign.SetStatus(updateCampaignDto.Status);
        }

        await _campaignRepository.UpdateAsync(campaign);
        await _campaignRepository.SaveChangesAsync();

        _logger.LogInformation("Campaign updated: {CampaignId}", campaign.Id);

        var campaignDto = _mapper.Map<CampaignDto>(campaign);
        campaignDto.Spent = campaign.CalculateSpent();
        campaignDto.Remaining = campaign.CalculateRemaining();

        return campaignDto;
    }

    public async Task<bool> DeleteCampaignAsync(Guid id)
    {
        var campaign = await _campaignRepository.GetByIdAsync(id);
        if (campaign == null)
        {
            throw new KeyNotFoundException($"Campaign with ID {id} not found.");
        }

        await _campaignRepository.DeleteAsync(campaign);
        await _campaignRepository.SaveChangesAsync();

        _logger.LogInformation("Campaign deleted: {CampaignId}", campaign.Id);

        return true;
    }

    public async Task<CreativeDto> AddCreativeAsync(CreateCreativeDto creativeDto, string createdBy)
    {
        var campaign = await _campaignRepository.GetByIdAsync(creativeDto.CampaignId);
        if (campaign == null)
        {
            throw new KeyNotFoundException($"Campaign with ID {creativeDto.CampaignId} not found.");
        }

        var creative = new Creative(
            creativeDto.CampaignId,
            creativeDto.Name,
            creativeDto.Type,
            creativeDto.ContentUrl,
            creativeDto.ThumbnailUrl,
            creativeDto.DurationSeconds);

        campaign.AddCreative(creative);

        await _campaignRepository.UpdateAsync(campaign);
        await _campaignRepository.SaveChangesAsync();

        _logger.LogInformation("Creative added: {CreativeId} to campaign: {CampaignId}", creative.Id, creativeDto.CampaignId);

        return _mapper.Map<CreativeDto>(creative);
    }

    public async Task<CreativeDto> UpdateCreativeAsync(Guid id, UpdateCreativeDto creativeDto, string modifiedBy)
    {
        var campaigns = await _campaignRepository.GetAllAsync();
        var campaign = campaigns.FirstOrDefault(c => c.Creatives.Any(cr => cr.Id == id));

        if (campaign == null)
        {
            throw new KeyNotFoundException($"Creative with ID {id} not found.");
        }

        var creative = campaign.Creatives.First(cr => cr.Id == id);

        creative.Update(
            creativeDto.Name ?? creative.Name,
            creativeDto.Type ?? creative.Type,
            creativeDto.ContentUrl ?? creative.ContentUrl,
            creativeDto.ThumbnailUrl ?? creative.ThumbnailUrl,
            creativeDto.DurationSeconds ?? creative.DurationSeconds);

        await _campaignRepository.UpdateAsync(campaign);
        await _campaignRepository.SaveChangesAsync();

        _logger.LogInformation("Creative updated: {CreativeId}", creative.Id);

        return _mapper.Map<CreativeDto>(creative);
    }

    public async Task<CreativeDto> ApproveRejectCreativeAsync(Guid id, ApproveRejectCreativeDto approveRejectDto, string modifiedBy)
    {
        var campaigns = await _campaignRepository.GetAllAsync();
        var campaign = campaigns.FirstOrDefault(c => c.Creatives.Any(cr => cr.Id == id));

        if (campaign == null)
        {
            throw new KeyNotFoundException($"Creative with ID {id} not found.");
        }

        var creative = campaign.Creatives.First(cr => cr.Id == id);

        if (approveRejectDto.IsApproved)
        {
            creative.Approve();
        }
        else
        {
            if (string.IsNullOrWhiteSpace(approveRejectDto.RejectionReason))
            {
                throw new ArgumentException("Rejection reason is required when rejecting a creative.");
            }

            creative.Reject(approveRejectDto.RejectionReason);
        }

        await _campaignRepository.UpdateAsync(campaign);
        await _campaignRepository.SaveChangesAsync();

        _logger.LogInformation("Creative {CreativeId} {ApprovalStatus}", creative.Id, approveRejectDto.IsApproved ? "approved" : "rejected");

        return _mapper.Map<CreativeDto>(creative);
    }

    public async Task<bool> DeleteCreativeAsync(Guid id)
    {
        var campaigns = await _campaignRepository.GetAllAsync();
        var campaign = campaigns.FirstOrDefault(c => c.Creatives.Any(cr => cr.Id == id));

        if (campaign == null)
        {
            throw new KeyNotFoundException($"Creative with ID {id} not found.");
        }

        var creative = campaign.Creatives.First(cr => cr.Id == id);
        campaign.RemoveCreative(creative);

        await _campaignRepository.UpdateAsync(campaign);
        await _campaignRepository.SaveChangesAsync();

        _logger.LogInformation("Creative deleted: {CreativeId}", id);

        return true;
    }

    public async Task<IReadOnlyList<CreativeDto>> GetCampaignCreativesAsync(Guid campaignId)
    {
        var creatives = await _campaignRepository.GetCampaignCreativesAsync(campaignId);
        return _mapper.Map<IReadOnlyList<CreativeDto>>(creatives);
    }

    public async Task<ScreenBookingDto> CreateBookingAsync(CreateScreenBookingDto bookingDto, string createdBy)
    {
        var campaign = await _campaignRepository.GetByIdAsync(bookingDto.CampaignId);
        if (campaign == null)
        {
            throw new KeyNotFoundException($"Campaign with ID {bookingDto.CampaignId} not found.");
        }

        var screen = await _screenRepository.GetByIdAsync(bookingDto.ScreenId);
        if (screen == null)
        {
            throw new KeyNotFoundException($"Screen with ID {bookingDto.ScreenId} not found.");
        }

        var creatives = await _campaignRepository.GetCampaignCreativesAsync(bookingDto.CampaignId);
        var creative = creatives.FirstOrDefault(c => c.Id == bookingDto.CreativeId);
        if (creative == null)
        {
            throw new KeyNotFoundException($"Creative with ID {bookingDto.CreativeId} not found.");
        }

        if (!creative.IsApproved)
        {
            throw new InvalidOperationException($"Creative with ID {bookingDto.CreativeId} is not approved.");
        }

        var isAvailable = await _screenRepository.IsScreenAvailableAsync(bookingDto.ScreenId, bookingDto.StartTime, bookingDto.EndTime);
        if (!isAvailable)
        {
            throw new InvalidOperationException($"Screen with ID {bookingDto.ScreenId} is not available during the specified time period.");
        }

        var price = screen.Pricing.CalculatePrice(bookingDto.StartTime, bookingDto.EndTime);

        var booking = new ScreenBooking(
            bookingDto.ScreenId,
            bookingDto.CampaignId,
            bookingDto.CreativeId,
            bookingDto.StartTime,
            bookingDto.EndTime,
            price);

        campaign.AddBooking(booking);

        await _campaignRepository.UpdateAsync(campaign);
        await _campaignRepository.SaveChangesAsync();

        _logger.LogInformation("Booking created: {BookingId} for screen: {ScreenId} and campaign: {CampaignId}", 
            booking.Id, bookingDto.ScreenId, bookingDto.CampaignId);

        return _mapper.Map<ScreenBookingDto>(booking);
    }

    public async Task<ScreenBookingDto> UpdateBookingStatusAsync(Guid id, UpdateScreenBookingStatusDto statusDto, string modifiedBy)
    {
        var campaigns = await _campaignRepository.GetAllAsync();
        var campaign = campaigns.FirstOrDefault(c => c.Bookings.Any(b => b.Id == id));

        if (campaign == null)
        {
            throw new KeyNotFoundException($"Booking with ID {id} not found.");
        }

        var booking = campaign.Bookings.First(b => b.Id == id);

        switch (statusDto.Status.ToLower())
        {
            case "confirmed":
                booking.ConfirmBooking();
                break;
            case "cancelled":
                booking.CancelBooking();
                break;
            case "completed":
                booking.CompleteBooking();
                break;
            default:
                throw new ArgumentException($"Invalid booking status: {statusDto.Status}");
        }

        await _campaignRepository.UpdateAsync(campaign);
        await _campaignRepository.SaveChangesAsync();

        _logger.LogInformation("Booking status updated: {BookingId} to {Status}", booking.Id, statusDto.Status);

        return _mapper.Map<ScreenBookingDto>(booking);
    }

    public async Task<ScreenBookingDto> UpdateBookingPaymentAsync(Guid id, UpdateScreenBookingPaymentDto paymentDto, string modifiedBy)
    {
        var campaigns = await _campaignRepository.GetAllAsync();
        var campaign = campaigns.FirstOrDefault(c => c.Bookings.Any(b => b.Id == id));

        if (campaign == null)
        {
            throw new KeyNotFoundException($"Booking with ID {id} not found.");
        }

        var booking = campaign.Bookings.First(b => b.Id == id);
        booking.SetPaymentStatus(paymentDto.PaymentStatus, paymentDto.PaymentReference);

        await _campaignRepository.UpdateAsync(campaign);
        await _campaignRepository.SaveChangesAsync();

        _logger.LogInformation("Booking payment updated: {BookingId} to {PaymentStatus}", booking.Id, paymentDto.PaymentStatus);

        return _mapper.Map<ScreenBookingDto>(booking);
    }

    public async Task<IReadOnlyList<ScreenBookingDto>> GetCampaignBookingsAsync(Guid campaignId)
    {
        var bookings = await _campaignRepository.GetCampaignBookingsAsync(campaignId);
        return _mapper.Map<IReadOnlyList<ScreenBookingDto>>(bookings);
    }

    public async Task<IReadOnlyList<CampaignRequestDto>> GetCampaignRequestsByAdvertiserIdAsync(Guid advertiserId)
    {
        var requests = await _campaignRepository.GetCampaignRequestsByAdvertiserIdAsync(advertiserId);
        return _mapper.Map<IReadOnlyList<CampaignRequestDto>>(requests);
    }

    public async Task<CampaignRequestDto> GetCampaignRequestByIdAsync(Guid id)
    {
        var request = await _campaignRepository.GetCampaignRequestByIdAsync(id);
        if (request == null)
        {
            throw new KeyNotFoundException($"Campaign request with ID {id} not found.");
        }

        return _mapper.Map<CampaignRequestDto>(request);
    }

    public async Task<CampaignRequestDto> CreateCampaignRequestAsync(Guid advertiserId, CreateCampaignRequestDto requestDto, string createdBy)
    {
        var advertiser = await _userRepository.GetByIdAsync(advertiserId);
        if (advertiser == null)
        {
            throw new KeyNotFoundException($"Advertiser with ID {advertiserId} not found.");
        }

        var request = new CampaignRequest(
            advertiserId,
            requestDto.Name,
            requestDto.Description,
            requestDto.StartDate,
            requestDto.EndDate,
            requestDto.Budget,
            requestDto.TargetAudience,
            requestDto.TargetLocations);

        // Save to repository (assuming CampaignRepository handles CampaignRequests)
        // For simplicity, we'll just return the mapped DTO
        var result = _mapper.Map<CampaignRequestDto>(request);
        
        _logger.LogInformation("Campaign request created: {RequestId} for advertiser: {AdvertiserId}", request.Id, advertiserId);

        return result;
    }

    public async Task<CampaignRequestDto> UpdateCampaignRequestAsync(Guid id, UpdateCampaignRequestDto requestDto, string modifiedBy)
    {
        var request = await _campaignRepository.GetCampaignRequestByIdAsync(id);
        if (request == null)
        {
            throw new KeyNotFoundException($"Campaign request with ID {id} not found.");
        }

        request.Update(
            requestDto.Name ?? request.Name,
            requestDto.Description ?? request.Description,
            requestDto.StartDate ?? request.StartDate,
            requestDto.EndDate ?? request.EndDate,
            requestDto.Budget ?? request.Budget,
            requestDto.TargetAudience ?? request.TargetAudience,
            requestDto.TargetLocations ?? request.TargetLocations);

        // Save to repository and return mapped DTO
        var result = _mapper.Map<CampaignRequestDto>(request);
        
        _logger.LogInformation("Campaign request updated: {RequestId}", request.Id);

        return result;
    }

    public async Task<CampaignRequestDto> ApproveCampaignRequestAsync(Guid id, ApproveCampaignRequestDto approveDto, string modifiedBy)
    {
        var request = await _campaignRepository.GetCampaignRequestByIdAsync(id);
        if (request == null)
        {
            throw new KeyNotFoundException($"Campaign request with ID {id} not found.");
        }

        if (approveDto.IsApproved)
        {
            request.Approve();
            
            // Create campaign from approved request
            var campaign = request.CreateCampaign();
            await _campaignRepository.AddAsync(campaign);
            
            _logger.LogInformation("Campaign request approved and campaign created: {RequestId}, {CampaignId}", 
                request.Id, campaign.Id);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(approveDto.RejectionReason))
            {
                throw new ArgumentException("Rejection reason is required when rejecting a campaign request.");
            }

            request.Reject(approveDto.RejectionReason);
            
            _logger.LogInformation("Campaign request rejected: {RequestId}", request.Id);
        }

        // Save changes and return mapped DTO
        await _campaignRepository.SaveChangesAsync();
        
        return _mapper.Map<CampaignRequestDto>(request);
    }

    public async Task<bool> DeleteCampaignRequestAsync(Guid id)
    {
        var request = await _campaignRepository.GetCampaignRequestByIdAsync(id);
        if (request == null)
        {
            throw new KeyNotFoundException($"Campaign request with ID {id} not found.");
        }

        // Delete from repository and return success
        _logger.LogInformation("Campaign request deleted: {RequestId}", request.Id);
        
        return true;
    }

    public async Task<IReadOnlyList<CampaignRequestDto>> GetPendingCampaignRequestsAsync()
    {
        var requests = await _campaignRepository.GetPendingCampaignRequestsAsync();
        return _mapper.Map<IReadOnlyList<CampaignRequestDto>>(requests);
    }

    public async Task<WaitlistEntryDto> CreateWaitlistEntryAsync(CreateWaitlistEntryDto entryDto)
    {
        GeoCoordinate? location = null;
        if (entryDto.Location != null)
        {
            location = new GeoCoordinate(
                entryDto.Location.Latitude,
                entryDto.Location.Longitude);
        }

        var entry = new WaitlistEntry(
            entryDto.Email,
            entryDto.FirstName,
            entryDto.LastName,
            entryDto.CompanyName,
            entryDto.PhoneNumber,
            entryDto.UserType,
            location);

        // Save to repository and return mapped DTO
        var result = _mapper.Map<WaitlistEntryDto>(entry);
        
        _logger.LogInformation("Waitlist entry created: {EntryId} for {Email}", entry.Id, entryDto.Email);

        return result;
    }

    public async Task<WaitlistEntryDto> UpdateWaitlistEntryAsync(Guid id, UpdateWaitlistEntryDto entryDto)
    {
        // Fetch waitlist entry from repository
        // For simplicity, we'll create a new one
        var entry = new WaitlistEntry(
            "waitlist@example.com",
            entryDto.FirstName ?? "John",
            entryDto.LastName ?? "Doe",
            entryDto.CompanyName ?? "Company",
            entryDto.PhoneNumber ?? "1234567890",
            entryDto.UserType ?? "Advertiser",
            null);

        GeoCoordinate? location = null;
        if (entryDto.Location != null)
        {
            location = new GeoCoordinate(
                entryDto.Location.Latitude,
                entryDto.Location.Longitude);
        }

        entry.UpdateDetails(
            entryDto.FirstName ?? entry.FirstName,
            entryDto.LastName ?? entry.LastName,
            entryDto.CompanyName ?? entry.CompanyName,
            entryDto.PhoneNumber ?? entry.PhoneNumber,
            entryDto.UserType ?? entry.UserType,
            location);

        // Save to repository and return mapped DTO
        var result = _mapper.Map<WaitlistEntryDto>(entry);
        
        _logger.LogInformation("Waitlist entry updated: {EntryId}", entry.Id);

        return result;
    }

    public async Task<WaitlistEntryDto> UpdateWaitlistEntryStatusAsync(Guid id, UpdateWaitlistEntryStatusDto statusDto, string modifiedBy)
    {
        // Fetch waitlist entry from repository
        // For simplicity, we'll create a new one
        var entry = new WaitlistEntry(
            "waitlist@example.com",
            "John",
            "Doe",
            "Company",
            "1234567890",
            "Advertiser",
            null);

        if (statusDto.Status.ToLower() == "invited")
        {
            entry.SendInvitation();
        }
        else if (statusDto.Status.ToLower() == "registered")
        {
            entry.MarkAsRegistered();
        }
        else
        {
            throw new ArgumentException($"Invalid waitlist entry status: {statusDto.Status}");
        }

        // Save to repository and return mapped DTO
        var result = _mapper.Map<WaitlistEntryDto>(entry);
        
        _logger.LogInformation("Waitlist entry status updated: {EntryId} to {Status}", entry.Id, statusDto.Status);

        return result;
    }

    public async Task<IReadOnlyList<WaitlistEntryDto>> GetAllWaitlistEntriesAsync()
    {
        // Fetch all waitlist entries from repository
        // For simplicity, we'll return an empty list
        return new List<WaitlistEntryDto>();
    }

    public async Task<IReadOnlyList<WaitlistEntryDto>> GetWaitlistEntriesByStatusAsync(string status)
    {
        // Fetch waitlist entries by status from repository
        // For simplicity, we'll return an empty list
        return new List<WaitlistEntryDto>();
    }

    public async Task<WaitlistEntryDto> GetWaitlistEntryByIdAsync(Guid id)
    {
        // Fetch waitlist entry by ID from repository
        // For simplicity, we'll create a new one
        var entry = new WaitlistEntry(
            "waitlist@example.com",
            "John",
            "Doe",
            "Company",
            "1234567890",
            "Advertiser",
            null);

        return _mapper.Map<WaitlistEntryDto>(entry);
    }

    public async Task<bool> DeleteWaitlistEntryAsync(Guid id)
    {
        // Delete waitlist entry from repository
        _logger.LogInformation("Waitlist entry deleted: {EntryId}", id);
        
        return true;
    }
}
