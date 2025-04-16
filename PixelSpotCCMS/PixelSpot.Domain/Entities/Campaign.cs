using PixelSpot.Domain.Common;

namespace PixelSpot.Domain.Entities;

public class Campaign : AuditableEntity
{
    public Guid AdvertiserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal Budget { get; private set; }
    public string Status { get; private set; } = string.Empty; // Draft, Active, Completed, Cancelled
    public string TargetAudience { get; private set; } = string.Empty;
    public string TargetLocations { get; private set; } = string.Empty;
    
    // Navigation properties
    public User Advertiser { get; private set; } = null!;
    public ICollection<Creative> Creatives { get; private set; } = new List<Creative>();
    public ICollection<ScreenBooking> Bookings { get; private set; } = new List<ScreenBooking>();
    
    private Campaign() { }
    
    public Campaign(Guid advertiserId, string name, string description, 
                  DateTime startDate, DateTime endDate, decimal budget, 
                  string targetAudience, string targetLocations)
    {
        if (advertiserId == Guid.Empty)
            throw new ArgumentException("Advertiser ID cannot be empty", nameof(advertiserId));
            
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Campaign name cannot be empty", nameof(name));
            
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
        Status = "Draft";
        TargetAudience = targetAudience;
        TargetLocations = targetLocations;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void Update(string name, string description, DateTime startDate, 
                     DateTime endDate, decimal budget, string targetAudience, 
                     string targetLocations)
    {
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
    
    public void SetStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be empty", nameof(status));
            
        if (status != "Draft" && status != "Active" && status != "Completed" && status != "Cancelled")
            throw new ArgumentException("Invalid status", nameof(status));
            
        Status = status;
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void AddCreative(Creative creative)
    {
        if (creative == null)
            throw new ArgumentNullException(nameof(creative));
            
        Creatives.Add(creative);
    }
    
    public void RemoveCreative(Creative creative)
    {
        if (creative == null)
            throw new ArgumentNullException(nameof(creative));
            
        Creatives.Remove(creative);
    }
    
    public void AddBooking(ScreenBooking booking)
    {
        if (booking == null)
            throw new ArgumentNullException(nameof(booking));
            
        Bookings.Add(booking);
    }
    
    public void RemoveBooking(ScreenBooking booking)
    {
        if (booking == null)
            throw new ArgumentNullException(nameof(booking));
            
        Bookings.Remove(booking);
    }
    
    public decimal CalculateSpent()
    {
        return Bookings.Sum(b => b.Price);
    }
    
    public decimal CalculateRemaining()
    {
        return Budget - CalculateSpent();
    }
    
    public bool IsActive()
    {
        return Status == "Active" && 
               DateTime.UtcNow >= StartDate && 
               DateTime.UtcNow <= EndDate;
    }
}
