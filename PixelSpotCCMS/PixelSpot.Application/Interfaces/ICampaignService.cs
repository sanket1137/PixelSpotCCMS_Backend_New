using PixelSpot.Application.DTOs;

namespace PixelSpot.Application.Interfaces;

public interface ICampaignService
{
    Task<CampaignDto> GetByIdAsync(Guid id);
    Task<IReadOnlyList<CampaignDto>> GetAllAsync();
    Task<IReadOnlyList<CampaignDto>> GetCampaignsByAdvertiserIdAsync(Guid advertiserId);
    Task<CampaignDto> CreateCampaignAsync(Guid advertiserId, CreateCampaignDto createCampaignDto, string createdBy);
    Task<CampaignDto> UpdateCampaignAsync(Guid id, UpdateCampaignDto updateCampaignDto, string modifiedBy);
    Task<bool> DeleteCampaignAsync(Guid id);
    Task<CreativeDto> AddCreativeAsync(CreateCreativeDto creativeDto, string createdBy);
    Task<CreativeDto> UpdateCreativeAsync(Guid id, UpdateCreativeDto creativeDto, string modifiedBy);
    Task<CreativeDto> ApproveRejectCreativeAsync(Guid id, ApproveRejectCreativeDto approveRejectDto, string modifiedBy);
    Task<bool> DeleteCreativeAsync(Guid id);
    Task<IReadOnlyList<CreativeDto>> GetCampaignCreativesAsync(Guid campaignId);
    Task<ScreenBookingDto> CreateBookingAsync(CreateScreenBookingDto bookingDto, string createdBy);
    Task<ScreenBookingDto> UpdateBookingStatusAsync(Guid id, UpdateScreenBookingStatusDto statusDto, string modifiedBy);
    Task<ScreenBookingDto> UpdateBookingPaymentAsync(Guid id, UpdateScreenBookingPaymentDto paymentDto, string modifiedBy);
    Task<IReadOnlyList<ScreenBookingDto>> GetCampaignBookingsAsync(Guid campaignId);
    Task<IReadOnlyList<CampaignRequestDto>> GetCampaignRequestsByAdvertiserIdAsync(Guid advertiserId);
    Task<CampaignRequestDto> GetCampaignRequestByIdAsync(Guid id);
    Task<CampaignRequestDto> CreateCampaignRequestAsync(Guid advertiserId, CreateCampaignRequestDto requestDto, string createdBy);
    Task<CampaignRequestDto> UpdateCampaignRequestAsync(Guid id, UpdateCampaignRequestDto requestDto, string modifiedBy);
    Task<CampaignRequestDto> ApproveCampaignRequestAsync(Guid id, ApproveCampaignRequestDto approveDto, string modifiedBy);
    Task<bool> DeleteCampaignRequestAsync(Guid id);
    Task<IReadOnlyList<CampaignRequestDto>> GetPendingCampaignRequestsAsync();
    Task<WaitlistEntryDto> CreateWaitlistEntryAsync(CreateWaitlistEntryDto entryDto);
    Task<WaitlistEntryDto> UpdateWaitlistEntryAsync(Guid id, UpdateWaitlistEntryDto entryDto);
    Task<WaitlistEntryDto> UpdateWaitlistEntryStatusAsync(Guid id, UpdateWaitlistEntryStatusDto statusDto, string modifiedBy);
    Task<IReadOnlyList<WaitlistEntryDto>> GetAllWaitlistEntriesAsync();
    Task<IReadOnlyList<WaitlistEntryDto>> GetWaitlistEntriesByStatusAsync(string status);
    Task<WaitlistEntryDto> GetWaitlistEntryByIdAsync(Guid id);
    Task<bool> DeleteWaitlistEntryAsync(Guid id);
}
