using AutoMapper;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;
using PixelSpot.Domain.Entities;
using PixelSpot.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace PixelSpot.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("This account is not active.");
        }

        var token = _jwtTokenService.GenerateToken(user);
        var tokenExpiration = DateTime.UtcNow.AddMinutes(_jwtTokenService.GetTokenExpirationMinutes());

        _logger.LogInformation("User logged in: {UserId}", user.Id);

        return new LoginResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            Token = token,
            TokenExpiration = tokenExpiration
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterRequestDto registerDto)
    {
        var emailExists = await _userRepository.EmailExistsAsync(registerDto.Email);
        if (emailExists)
        {
            throw new InvalidOperationException($"Email {registerDto.Email} is already in use.");
        }

        if (registerDto.Password != registerDto.ConfirmPassword)
        {
            throw new ArgumentException("Password and confirmation password do not match.");
        }

        var passwordHash = HashPassword(registerDto.Password);

        var user = new User(
            registerDto.Email,
            passwordHash,
            registerDto.FirstName,
            registerDto.LastName,
            registerDto.CompanyName,
            registerDto.PhoneNumber,
            registerDto.Role);

        user = await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("User registered: {UserId}", user.Id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordDto)
    {
        var user = await _userRepository.GetByEmailAsync(forgotPasswordDto.Email);
        if (user == null)
        {
            // For security reasons, don't reveal that the email doesn't exist
            _logger.LogWarning("Password reset requested for non-existent email: {Email}", forgotPasswordDto.Email);
            return true;
        }

        // In a real application, generate a reset token and send an email with a reset link
        // For simplicity, we'll just log the action
        _logger.LogInformation("Password reset requested for user: {UserId}", user.Id);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordDto)
    {
        var user = await _userRepository.GetByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with email {resetPasswordDto.Email} not found.");
        }

        // In a real application, validate the reset token
        // For simplicity, we'll just check that the passwords match

        if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
        {
            throw new ArgumentException("Password and confirmation password do not match.");
        }

        var newPasswordHash = HashPassword(resetPasswordDto.NewPassword);
        user.ChangePassword(newPasswordHash);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("Password reset completed for user: {UserId}", user.Id);

        return true;
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenDto)
    {
        var userId = _jwtTokenService.ValidateTokenAndGetUserId(refreshTokenDto.Token);
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Invalid or expired token.");
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("This account is not active.");
        }

        var newToken = _jwtTokenService.GenerateToken(user);
        var tokenExpiration = DateTime.UtcNow.AddMinutes(_jwtTokenService.GetTokenExpirationMinutes());

        _logger.LogInformation("Token refreshed for user: {UserId}", user.Id);

        return new LoginResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            Token = newToken,
            TokenExpiration = tokenExpiration
        };
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var userId = _jwtTokenService.ValidateTokenAndGetUserId(token);
        if (userId == Guid.Empty)
        {
            return false;
        }

        var user = await _userRepository.GetByIdAsync(userId);
        return user != null && user.IsActive;
    }

    public async Task<Guid> GetUserIdFromTokenAsync(string token)
    {
        return _jwtTokenService.ValidateTokenAndGetUserId(token);
    }

    public async Task<string> GetUserRoleFromTokenAsync(string token)
    {
        return _jwtTokenService.ValidateTokenAndGetRole(token);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha256.ComputeHash(passwordBytes);
        return Convert.ToBase64String(hashBytes);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        var newHash = HashPassword(password);
        return newHash == passwordHash;
    }
}

// Interface moved to PixelSpot.Application.Interfaces.IJwtTokenService
