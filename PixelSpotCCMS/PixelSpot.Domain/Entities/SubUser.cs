using PixelSpot.Domain.Common;

namespace PixelSpot.Domain.Entities;

public class SubUser : AuditableEntity
{
    public Guid UserId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public User User { get; private set; } = null!;
    public ICollection<Permission> Permissions { get; private set; } = new List<Permission>();
    
    private SubUser() { }
    
    public SubUser(Guid userId, string email, string passwordHash, string firstName, string lastName)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
            
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
            
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
        
        UserId = userId;
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void Update(string firstName, string lastName, string email)
    {
        if (!string.IsNullOrWhiteSpace(firstName))
            FirstName = firstName;
            
        if (!string.IsNullOrWhiteSpace(lastName))
            LastName = lastName;
            
        if (!string.IsNullOrWhiteSpace(email))
            Email = email;
            
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
    
    public void AddPermission(Permission permission)
    {
        if (permission == null)
            throw new ArgumentNullException(nameof(permission));
            
        Permissions.Add(permission);
    }
    
    public void RemovePermission(Permission permission)
    {
        if (permission == null)
            throw new ArgumentNullException(nameof(permission));
            
        Permissions.Remove(permission);
    }
    
    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }
}
