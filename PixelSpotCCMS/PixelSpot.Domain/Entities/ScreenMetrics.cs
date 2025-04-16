using PixelSpot.Domain.Common;

namespace PixelSpot.Domain.Entities;

public class ScreenMetrics : Entity
{
    public Guid ScreenId { get; private set; }
    public DateTime Date { get; private set; }
    public int EstimatedViews { get; private set; }
    public int EstimatedImpressions { get; private set; }
    public int EstimatedEngagement { get; private set; }
    public string AudienceDemographics { get; private set; } = string.Empty;
    
    // Navigation property
    public Screen Screen { get; private set; } = null!;
    
    private ScreenMetrics() { }
    
    public ScreenMetrics(Guid screenId, DateTime date, int estimatedViews, 
                       int estimatedImpressions, int estimatedEngagement, 
                       string audienceDemographics)
    {
        if (screenId == Guid.Empty)
            throw new ArgumentException("Screen ID cannot be empty", nameof(screenId));
            
        if (estimatedViews < 0)
            throw new ArgumentException("Estimated views cannot be negative", nameof(estimatedViews));
            
        if (estimatedImpressions < 0)
            throw new ArgumentException("Estimated impressions cannot be negative", nameof(estimatedImpressions));
            
        if (estimatedEngagement < 0)
            throw new ArgumentException("Estimated engagement cannot be negative", nameof(estimatedEngagement));
        
        ScreenId = screenId;
        Date = date.Date; // Store only the date part
        EstimatedViews = estimatedViews;
        EstimatedImpressions = estimatedImpressions;
        EstimatedEngagement = estimatedEngagement;
        AudienceDemographics = audienceDemographics;
    }
    
    public void Update(int estimatedViews, int estimatedImpressions, 
                     int estimatedEngagement, string audienceDemographics)
    {
        if (estimatedViews >= 0)
            EstimatedViews = estimatedViews;
            
        if (estimatedImpressions >= 0)
            EstimatedImpressions = estimatedImpressions;
            
        if (estimatedEngagement >= 0)
            EstimatedEngagement = estimatedEngagement;
            
        AudienceDemographics = audienceDemographics;
    }
}
