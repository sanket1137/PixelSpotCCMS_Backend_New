using PixelSpot.Domain.Common;
using PixelSpot.Domain.ValueObjects;

namespace PixelSpot.Domain.Entities;

public class WaitlistEntry : AuditableEntity
{
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string CompanyName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string UserType { get; private set; } = string.Empty; // Advertiser, ScreenOwner, Both
    public string Status { get; private set; } = string.Empty; // Pending, Invited, Registered
    public DateTime? InvitationSentDate { get; private set; }
    public DateTime? RegistrationDate { get; private set; }
    public GeoCoordinate? Location { get; private set; }
    
    private WaitlistEntry() { }
    
    public WaitlistEntry(string email, string firstName, string lastName, 
                       string companyName, string phoneNumber, string userType,
                       GeoCoordinate? location)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
            
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            
        if (string.IsNullOrWhiteSpace(userType))
            throw new ArgumentException("User type cannot be empty", nameof(userType));
            
        if (userType != "Advertiser" && userType != "ScreenOwner" && userType != "Both")
            throw new ArgumentException("Invalid user type", nameof(userType));
        
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        CompanyName = companyName;
        PhoneNumber = phoneNumber;
        UserType = userType;
        Status = "Pending";
        Location = location;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void SendInvitation()
    {
        Status = "Invited";
        InvitationSentDate = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void MarkAsRegistered()
    {
        Status = "Registered";
        RegistrationDate = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void UpdateDetails(string firstName, string lastName, string companyName, 
                           string phoneNumber, string userType, GeoCoordinate? location)
    {
        if (!string.IsNullOrWhiteSpace(firstName))
            FirstName = firstName;
            
        if (!string.IsNullOrWhiteSpace(lastName))
            LastName = lastName;
            
        CompanyName = companyName;
        PhoneNumber = phoneNumber;
        
        if (!string.IsNullOrWhiteSpace(userType) && 
            (userType == "Advertiser" || userType == "ScreenOwner" || userType == "Both"))
            UserType = userType;
            
        if (location != null)
            Location = location;
            
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }
}
