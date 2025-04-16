using Microsoft.EntityFrameworkCore;
using PixelSpot.Domain.Entities;
using PixelSpot.Domain.Interfaces;
using PixelSpot.Infrastructure.Data.Contexts;

namespace PixelSpot.Infrastructure.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(string role)
    {
        return await _dbContext.Users
            .Where(u => u.Role == role)
            .ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email == email) ||
               await _dbContext.SubUsers.AnyAsync(su => su.Email == email);
    }

    public async Task<IReadOnlyList<SubUser>> GetSubUsersByUserIdAsync(Guid userId)
    {
        return await _dbContext.SubUsers
            .Where(su => su.UserId == userId)
            .Include(su => su.Permissions)
            .ToListAsync();
    }

    public async Task<User?> GetUserWithDetailsAsync(Guid id)
    {
        return await _dbContext.Users
            .Include(u => u.SubUsers)
                .ThenInclude(su => su.Permissions)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}
