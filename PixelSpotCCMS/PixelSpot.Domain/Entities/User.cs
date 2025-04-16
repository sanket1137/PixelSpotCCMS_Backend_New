using PixelSpot.Domain.Common;
using PixelSpot.Domain.ValueObjects;

namespace PixelSpot.Domain.Entities;

public class User : AuditableEntity
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string CompanyName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public bool IsVerified { get; private set; }
    public bool IsActive { get; private set; }
    public string Role { get; private set; } = string.Empty;
    public BankDetails? BankDetails { get; private set; }
    public string ProfileImageUrl { get; private set; } = string.Empty;
    
    // Navigation properties
    public ICollection<SubUser> SubUsers { get; private set; } = new List<SubUser>();
    public ICollection<Screen> Screens { get; private set; } = new List<Screen>();
    public ICollection<Campaign> Campaigns { get; private set; } = new List<Campaign>();
    
    private User() { }
    
    public User(string email, string passwordHash, string firstName, string lastName, 
               string companyName, string phoneNumber, string role)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
            
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
            
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
        
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        CompanyName = companyName;
        PhoneNumber = phoneNumber;
        Role = role;
        IsVerified = false;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void Update(string firstName, string lastName, string companyName, 
                     string phoneNumber, string profileImageUrl)
    {
        if (!string.IsNullOrWhiteSpace(firstName))
            FirstName = firstName;
            
        if (!string.IsNullOrWhiteSpace(lastName))
            LastName = lastName;
            
        CompanyName = companyName;
        PhoneNumber = phoneNumber;
        
        if (!string.IsNullOrWhiteSpace(profileImageUrl))
            ProfileImageUrl = profileImageUrl;
            
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void SetBankDetails(BankDetails bankDetails)
    {
        BankDetails = bankDetails ?? throw new ArgumentNullException(nameof(bankDetails));
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void SetVerificationStatus(bool isVerified)
    {
        IsVerified = isVerified;
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));
            
        PasswordHash = newPasswordHash;
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public void AddSubUser(SubUser subUser)
    {
        if (subUser == null)
            throw new ArgumentNullException(nameof(subUser));
            
        SubUsers.Add(subUser);
    }
    
    public void RemoveSubUser(SubUser subUser)
    {
        if (subUser == null)
            throw new ArgumentNullException(nameof(subUser));
            
        SubUsers.Remove(subUser);
    }
    
    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }
}
