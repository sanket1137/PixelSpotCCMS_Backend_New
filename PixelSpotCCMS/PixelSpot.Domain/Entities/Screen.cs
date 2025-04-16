using PixelSpot.Domain.Common;
using PixelSpot.Domain.ValueObjects;

namespace PixelSpot.Domain.Entities;

public class Screen : AuditableEntity
{
    public Guid OwnerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty; // Digital, LED, Billboard, etc.
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public GeoCoordinate Location { get; private set; } = null!;
    public ScreenSize Size { get; private set; } = null!;
    public string ImageUrl { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public bool IsVerified { get; private set; }
    
    // Navigation properties
    public User Owner { get; private set; } = null!;
    public ScreenPricing Pricing { get; private set; } = null!;
    public ICollection<ScreenAvailability> Availabilities { get; private set; } = new List<ScreenAvailability>();
    public ICollection<ScreenMetrics> Metrics { get; private set; } = new List<ScreenMetrics>();
    public ICollection<ScreenBooking> Bookings { get; private set; } = new List<ScreenBooking>();
    
    private Screen() { }
    
    public Screen(Guid ownerId, string name, string description, string type, 
                string address, string city, string state, string country, string postalCode,
                GeoCoordinate location, ScreenSize size, string imageUrl)
    {
        if (ownerId == Guid.Empty)
            throw new ArgumentException("Owner ID cannot be empty", nameof(ownerId));
            
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Screen name cannot be empty", nameof(name));
            
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Screen type cannot be empty", nameof(type));
            
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be empty", nameof(address));
            
        if (location == null)
            throw new ArgumentNullException(nameof(location));
            
        if (size == null)
            throw new ArgumentNullException(nameof(size));
        
        OwnerId = ownerId;
        Name = name;
        Description = description;
        Type = type;
        Address = address;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        Location = location;
        Size = size;
        ImageUrl = imageUrl;
        IsActive = true;
        IsVerified = false;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void Update(string name, string description, string type,
                     string address, string city, string state, string country, string postalCode,
                     GeoCoordinate location, ScreenSize size, string imageUrl)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;
            
        Description = description;
        
        if (!string.IsNullOrWhiteSpace(type))
            Type = type;
            
        if (!string.IsNullOrWhiteSpace(address))
            Address = address;
            
        if (!string.IsNullOrWhiteSpace(city))
            City = city;
            
        if (!string.IsNullOrWhiteSpace(state))
            State = state;
            
        if (!string.IsNullOrWhiteSpace(country))
            Country = country;
            
        if (!string.IsNullOrWhiteSpace(postalCode))
            PostalCode = postalCode;
            
        if (location != null)
            Location = location;
            
        if (size != null)
            Size = size;
            
        if (!string.IsNullOrWhiteSpace(imageUrl))
            ImageUrl = imageUrl;
            
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void SetVerificationStatus(bool isVerified)
    {
        IsVerified = isVerified;
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void SetPricing(ScreenPricing pricing)
    {
        Pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
    }
    
    public void AddAvailability(ScreenAvailability availability)
    {
        if (availability == null)
            throw new ArgumentNullException(nameof(availability));
            
        Availabilities.Add(availability);
    }
    
    public void RemoveAvailability(ScreenAvailability availability)
    {
        if (availability == null)
            throw new ArgumentNullException(nameof(availability));
            
        Availabilities.Remove(availability);
    }
    
    public bool IsAvailable(DateTime startTime, DateTime endTime)
    {
        // Check if the screen is active and verified
        if (!IsActive || !IsVerified)
            return false;
            
        // Check against availabilities
        foreach (var availability in Availabilities)
        {
            if (availability.IsTimeSlotAvailable(startTime, endTime))
                return true;
        }
        
        return false;
    }
    
    public string GetFullAddress()
    {
        return $"{Address}, {City}, {State} {PostalCode}, {Country}";
    }
}
