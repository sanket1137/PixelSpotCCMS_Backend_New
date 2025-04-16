using PixelSpot.Domain.Entities;

namespace PixelSpot.Domain.Interfaces;

public interface ICampaignRepository : IRepository<Campaign>
{
    Task<IReadOnlyList<Campaign>> GetCampaignsByAdvertiserIdAsync(Guid advertiserId);
    Task<Campaign?> GetCampaignWithDetailsAsync(Guid id);
    Task<IReadOnlyList<Creative>> GetCampaignCreativesAsync(Guid campaignId);
    Task<IReadOnlyList<ScreenBooking>> GetCampaignBookingsAsync(Guid campaignId);
    Task<IReadOnlyList<Campaign>> GetActiveCampaignsAsync();
    Task<IReadOnlyList<CampaignRequest>> GetCampaignRequestsByAdvertiserIdAsync(Guid advertiserId);
    Task<CampaignRequest?> GetCampaignRequestByIdAsync(Guid id);
    Task<IReadOnlyList<CampaignRequest>> GetPendingCampaignRequestsAsync();
}
