using PixelSpot.Domain.Common;

namespace PixelSpot.Domain.Entities;

public class CampaignRequest : AuditableEntity
{
    public Guid AdvertiserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal Budget { get; private set; }
    public string TargetAudience { get; private set; } = string.Empty;
    public string TargetLocations { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty; // Pending, Approved, Rejected
    public string RejectionReason { get; private set; } = string.Empty;
    
    // Navigation property
    public User Advertiser { get; private set; } = null!;
    
    private CampaignRequest() { }
    
    public CampaignRequest(Guid advertiserId, string name, string description, 
                          DateTime startDate, DateTime endDate, decimal budget, 
                          string targetAudience, string targetLocations)
    {
        if (advertiserId == Guid.Empty)
            throw new ArgumentException("Advertiser ID cannot be empty", nameof(advertiserId));
            
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Campaign request name cannot be empty", nameof(name));
            
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date", nameof(startDate));
            
        if (budget < 0)
            throw new ArgumentException("Budget cannot be negative", nameof(budget));
        
        AdvertiserId = advertiserId;
        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        Budget = budget;
        TargetAudience = targetAudience;
        TargetLocations = targetLocations;
        Status = "Pending";
        CreatedAt = DateTime.UtcNow;
    }
    
    public void Update(string name, string description, DateTime startDate, 
                     DateTime endDate, decimal budget, string targetAudience, 
                     string targetLocations)
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Cannot update a request that is not pending");
            
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;
            
        Description = description;
        
        if (startDate < endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
        
        if (budget >= 0)
            Budget = budget;
            
        TargetAudience = targetAudience;
        TargetLocations = targetLocations;
        
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void Approve()
    {
        Status = "Approved";
        RejectionReason = string.Empty;
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void Reject(string reason)
    {
        Status = "Rejected";
        RejectionReason = reason;
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public Campaign CreateCampaign()
    {
        if (Status != "Approved")
            throw new InvalidOperationException("Cannot create a campaign from a request that is not approved");
            
        return new Campaign(
            AdvertiserId,
            Name,
            Description,
            StartDate,
            EndDate,
            Budget,
            TargetAudience,
            TargetLocations
        );
    }
}
