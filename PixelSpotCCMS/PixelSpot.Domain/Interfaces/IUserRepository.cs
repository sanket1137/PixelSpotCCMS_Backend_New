using PixelSpot.Domain.Entities;

namespace PixelSpot.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IReadOnlyList<User>> GetUsersByRoleAsync(string role);
    Task<bool> EmailExistsAsync(string email);
    Task<IReadOnlyList<SubUser>> GetSubUsersByUserIdAsync(Guid userId);
    Task<User?> GetUserWithDetailsAsync(Guid id);
}
