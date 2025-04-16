using PixelSpot.Domain.Common;

namespace PixelSpot.Domain.Entities;

public class ScreenBooking : AuditableEntity
{
    public Guid ScreenId { get; private set; }
    public Guid CampaignId { get; private set; }
    public Guid CreativeId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public decimal Price { get; private set; }
    public string Status { get; private set; } = string.Empty; // Pending, Confirmed, Cancelled, Completed
    public string PaymentStatus { get; private set; } = string.Empty; // Pending, Paid, Refunded
    public string PaymentReference { get; private set; } = string.Empty;
    
    // Navigation properties
    public Screen Screen { get; private set; } = null!;
    public Campaign Campaign { get; private set; } = null!;
    public Creative Creative { get; private set; } = null!;
    
    private ScreenBooking() { }
    
    public ScreenBooking(Guid screenId, Guid campaignId, Guid creativeId, 
                       DateTime startTime, DateTime endTime, decimal price)
    {
        if (screenId == Guid.Empty)
            throw new ArgumentException("Screen ID cannot be empty", nameof(screenId));
            
        if (campaignId == Guid.Empty)
            throw new ArgumentException("Campaign ID cannot be empty", nameof(campaignId));
            
        if (creativeId == Guid.Empty)
            throw new ArgumentException("Creative ID cannot be empty", nameof(creativeId));
            
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time", nameof(startTime));
            
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));
        
        ScreenId = screenId;
        CampaignId = campaignId;
        CreativeId = creativeId;
        StartTime = startTime;
        EndTime = endTime;
        Price = price;
        Status = "Pending";
        PaymentStatus = "Pending";
        CreatedAt = DateTime.UtcNow;
    }
    
    public void ConfirmBooking()
    {
        Status = "Confirmed";
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void CancelBooking()
    {
        Status = "Cancelled";
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void CompleteBooking()
    {
        Status = "Completed";
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void SetPaymentStatus(string paymentStatus, string paymentReference)
    {
        if (string.IsNullOrWhiteSpace(paymentStatus))
            throw new ArgumentException("Payment status cannot be empty", nameof(paymentStatus));
            
        if (paymentStatus != "Pending" && paymentStatus != "Paid" && paymentStatus != "Refunded")
            throw new ArgumentException("Invalid payment status", nameof(paymentStatus));
            
        PaymentStatus = paymentStatus;
        
        if (!string.IsNullOrWhiteSpace(paymentReference))
            PaymentReference = paymentReference;
            
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public bool IsActive()
    {
        return Status == "Confirmed" && 
               DateTime.UtcNow >= StartTime && 
               DateTime.UtcNow <= EndTime;
    }
    
    public bool IsOverlapping(DateTime otherStartTime, DateTime otherEndTime)
    {
        return (StartTime <= otherEndTime && EndTime >= otherStartTime);
    }
}
