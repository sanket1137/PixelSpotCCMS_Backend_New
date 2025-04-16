using PixelSpot.Domain.Entities;

namespace PixelSpot.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    Guid ValidateTokenAndGetUserId(string token);
    string ValidateTokenAndGetRole(string token);
    int GetTokenExpirationMinutes();
}