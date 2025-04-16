using PixelSpot.Domain.Common;

namespace PixelSpot.Domain.Entities;

public class Permission : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    
    // Navigation properties
    public ICollection<SubUser> SubUsers { get; private set; } = new List<SubUser>();
    
    private Permission() { }
    
    public Permission(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be empty", nameof(name));
        
        Name = name;
        Description = description;
    }
    
    public void Update(string name, string description)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;
            
        Description = description;
    }
}
