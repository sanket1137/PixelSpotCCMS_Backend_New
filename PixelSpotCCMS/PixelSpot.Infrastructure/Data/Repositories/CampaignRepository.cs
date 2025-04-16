using Microsoft.EntityFrameworkCore;
using PixelSpot.Domain.Entities;
using PixelSpot.Domain.Interfaces;
using PixelSpot.Infrastructure.Data.Contexts;

namespace PixelSpot.Infrastructure.Data.Repositories;

public class CampaignRepository : BaseRepository<Campaign>, ICampaignRepository
{
    public CampaignRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Campaign>> GetCampaignsByAdvertiserIdAsync(Guid advertiserId)
    {
        return await _dbContext.Campaigns
            .Where(c => c.AdvertiserId == advertiserId)
            .Include(c => c.Advertiser)
            .Include(c => c.Creatives)
            .Include(c => c.Bookings)
                .ThenInclude(b => b.Screen)
            .ToListAsync();
    }

    public async Task<Campaign?> GetCampaignWithDetailsAsync(Guid id)
    {
        return await _dbContext.Campaigns
            .Include(c => c.Advertiser)
            .Include(c => c.Creatives)
            .Include(c => c.Bookings)
                .ThenInclude(b => b.Screen)
            .Include(c => c.Bookings)
                .ThenInclude(b => b.Creative)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IReadOnlyList<Creative>> GetCampaignCreativesAsync(Guid campaignId)
    {
        return await _dbContext.Creatives
            .Where(c => c.CampaignId == campaignId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ScreenBooking>> GetCampaignBookingsAsync(Guid campaignId)
    {
        return await _dbContext.ScreenBookings
            .Where(b => b.CampaignId == campaignId)
            .Include(b => b.Screen)
            .Include(b => b.Creative)
            .OrderBy(b => b.StartTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Campaign>> GetActiveCampaignsAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbContext.Campaigns
            .Where(c => c.Status == "Active" && c.StartDate <= now && c.EndDate >= now)
            .Include(c => c.Advertiser)
            .Include(c => c.Creatives.Where(cr => cr.IsApproved))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<CampaignRequest>> GetCampaignRequestsByAdvertiserIdAsync(Guid advertiserId)
    {
        return await _dbContext.CampaignRequests
            .Where(r => r.AdvertiserId == advertiserId)
            .Include(r => r.Advertiser)
            .ToListAsync();
    }

    public async Task<CampaignRequest?> GetCampaignRequestByIdAsync(Guid id)
    {
        return await _dbContext.CampaignRequests
            .Include(r => r.Advertiser)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IReadOnlyList<CampaignRequest>> GetPendingCampaignRequestsAsync()
    {
        return await _dbContext.CampaignRequests
            .Where(r => r.Status == "Pending")
            .Include(r => r.Advertiser)
            .ToListAsync();
    }
}
