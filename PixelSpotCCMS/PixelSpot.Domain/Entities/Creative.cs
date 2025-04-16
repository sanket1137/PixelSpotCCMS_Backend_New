using PixelSpot.Domain.Common;

namespace PixelSpot.Domain.Entities;

public class Creative : AuditableEntity
{
    public Guid CampaignId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty; // Image, Video, HTML
    public string ContentUrl { get; private set; } = string.Empty;
    public string ThumbnailUrl { get; private set; } = string.Empty;
    public int DurationSeconds { get; private set; }
    public bool IsApproved { get; private set; }
    public string RejectionReason { get; private set; } = string.Empty;
    
    // Navigation property
    public Campaign Campaign { get; private set; } = null!;
    
    private Creative() { }
    
    public Creative(Guid campaignId, string name, string type, string contentUrl, 
                  string thumbnailUrl, int durationSeconds)
    {
        if (campaignId == Guid.Empty)
            throw new ArgumentException("Campaign ID cannot be empty", nameof(campaignId));
            
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Creative name cannot be empty", nameof(name));
            
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Creative type cannot be empty", nameof(type));
            
        if (string.IsNullOrWhiteSpace(contentUrl))
            throw new ArgumentException("Content URL cannot be empty", nameof(contentUrl));
            
        if (durationSeconds <= 0)
            throw new ArgumentException("Duration must be greater than zero", nameof(durationSeconds));
        
        CampaignId = campaignId;
        Name = name;
        Type = type;
        ContentUrl = contentUrl;
        ThumbnailUrl = thumbnailUrl;
        DurationSeconds = durationSeconds;
        IsApproved = false;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void Update(string name, string type, string contentUrl, 
                     string thumbnailUrl, int durationSeconds)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;
            
        if (!string.IsNullOrWhiteSpace(type))
            Type = type;
            
        if (!string.IsNullOrWhiteSpace(contentUrl))
            ContentUrl = contentUrl;
            
        ThumbnailUrl = thumbnailUrl;
        
        if (durationSeconds > 0)
            DurationSeconds = durationSeconds;
            
        // Reset approval status when content is updated
        IsApproved = false;
        RejectionReason = string.Empty;
        
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void Approve()
    {
        IsApproved = true;
        RejectionReason = string.Empty;
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void Reject(string reason)
    {
        IsApproved = false;
        RejectionReason = reason;
        LastModifiedAt = DateTime.UtcNow;
    }
}
