using PixelSpot.Application.DTOs;

namespace PixelSpot.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto);
    Task<UserDto> RegisterAsync(RegisterRequestDto registerDto);
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordDto);
    Task<bool> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordDto);
    Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenDto);
    Task<bool> ValidateTokenAsync(string token);
    Task<Guid> GetUserIdFromTokenAsync(string token);
    Task<string> GetUserRoleFromTokenAsync(string token);
}
