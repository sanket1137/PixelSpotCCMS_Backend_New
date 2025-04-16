namespace PixelSpot.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; protected set; }
    public DateTime? LastModifiedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public string LastModifiedBy { get; protected set; } = string.Empty;
}
