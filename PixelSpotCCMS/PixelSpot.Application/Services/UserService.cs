using AutoMapper;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;
using PixelSpot.Domain.Entities;
using PixelSpot.Domain.Interfaces;
using PixelSpot.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace PixelSpot.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetUserWithDetailsAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<UserDto>>(users);
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersByRoleAsync(string role)
    {
        var users = await _userRepository.GetUsersByRoleAsync(role);
        return _mapper.Map<IReadOnlyList<UserDto>>(users);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, string createdBy)
    {
        var emailExists = await _userRepository.EmailExistsAsync(createUserDto.Email);
        if (emailExists)
        {
            throw new InvalidOperationException($"Email {createUserDto.Email} is already in use.");
        }

        var passwordHash = HashPassword(createUserDto.Password);

        var user = new User(
            createUserDto.Email,
            passwordHash,
            createUserDto.FirstName,
            createUserDto.LastName,
            createUserDto.CompanyName,
            createUserDto.PhoneNumber,
            createUserDto.Role);

        user = await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("User created: {UserId}", user.Id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto, string modifiedBy)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        user.Update(
            updateUserDto.FirstName ?? user.FirstName,
            updateUserDto.LastName ?? user.LastName,
            updateUserDto.CompanyName ?? user.CompanyName,
            updateUserDto.PhoneNumber ?? user.PhoneNumber,
            updateUserDto.ProfileImageUrl ?? user.ProfileImageUrl);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("User updated: {UserId}", user.Id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateBankDetailsAsync(Guid id, UpdateBankDetailsDto bankDetailsDto, string modifiedBy)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        var bankDetails = new BankDetails(
            bankDetailsDto.AccountHolderName,
            bankDetailsDto.BankName,
            bankDetailsDto.AccountNumber,
            bankDetailsDto.RoutingNumber);

        user.SetBankDetails(bankDetails);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("Bank details updated for user: {UserId}", user.Id);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto changePasswordDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        // Verify current password
        if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
        {
            throw new InvalidOperationException("Current password is incorrect.");
        }

        var newPasswordHash = HashPassword(changePasswordDto.NewPassword);
        user.ChangePassword(newPasswordHash);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("Password changed for user: {UserId}", user.Id);

        return true;
    }

    public async Task<bool> SetUserActiveStatusAsync(Guid id, bool isActive, string modifiedBy)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        user.SetActiveStatus(isActive);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("User {UserId} active status set to {IsActive}", user.Id, isActive);

        return true;
    }

    public async Task<bool> SetUserVerificationStatusAsync(Guid id, bool isVerified, string modifiedBy)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        user.SetVerificationStatus(isVerified);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("User {UserId} verification status set to {IsVerified}", user.Id, isVerified);

        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("User deleted: {UserId}", user.Id);

        return true;
    }

    public async Task<IReadOnlyList<SubUserDto>> GetSubUsersByUserIdAsync(Guid userId)
    {
        var subUsers = await _userRepository.GetSubUsersByUserIdAsync(userId);
        return _mapper.Map<IReadOnlyList<SubUserDto>>(subUsers);
    }

    public async Task<SubUserDto> GetSubUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetUserWithDetailsAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"SubUser with ID {id} not found.");
        }

        var subUser = user.SubUsers.FirstOrDefault(su => su.Id == id);
        if (subUser == null)
        {
            throw new KeyNotFoundException($"SubUser with ID {id} not found.");
        }

        return _mapper.Map<SubUserDto>(subUser);
    }

    public async Task<SubUserDto> CreateSubUserAsync(Guid userId, CreateSubUserDto createSubUserDto, string createdBy)
    {
        var user = await _userRepository.GetUserWithDetailsAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        var emailExists = await _userRepository.EmailExistsAsync(createSubUserDto.Email);
        if (emailExists)
        {
            throw new InvalidOperationException($"Email {createSubUserDto.Email} is already in use.");
        }

        var passwordHash = HashPassword(createSubUserDto.Password);

        var subUser = new SubUser(
            userId,
            createSubUserDto.Email,
            passwordHash,
            createSubUserDto.FirstName,
            createSubUserDto.LastName);

        user.AddSubUser(subUser);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("SubUser created: {SubUserId} for user: {UserId}", subUser.Id, userId);

        return _mapper.Map<SubUserDto>(subUser);
    }

    public async Task<SubUserDto> UpdateSubUserAsync(Guid id, UpdateSubUserDto updateSubUserDto, string modifiedBy)
    {
        var user = await _userRepository.GetUserWithDetailsAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        var subUser = user.SubUsers.FirstOrDefault(su => su.Id == id);
        if (subUser == null)
        {
            throw new KeyNotFoundException($"SubUser with ID {id} not found.");
        }

        subUser.Update(
            updateSubUserDto.FirstName ?? subUser.FirstName,
            updateSubUserDto.LastName ?? subUser.LastName,
            updateSubUserDto.Email ?? subUser.Email);

        if (updateSubUserDto.IsActive.HasValue)
        {
            subUser.SetActiveStatus(updateSubUserDto.IsActive.Value);
        }

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("SubUser updated: {SubUserId}", subUser.Id);

        return _mapper.Map<SubUserDto>(subUser);
    }

    public async Task<bool> DeleteSubUserAsync(Guid id)
    {
        var user = await _userRepository.GetUserWithDetailsAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        var subUser = user.SubUsers.FirstOrDefault(su => su.Id == id);
        if (subUser == null)
        {
            throw new KeyNotFoundException($"SubUser with ID {id} not found.");
        }

        user.RemoveSubUser(subUser);

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("SubUser deleted: {SubUserId}", subUser.Id);

        return true;
    }

    public async Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync()
    {
        // This would typically fetch permissions from a repository
        // For simplicity, we'll return a static list
        var permissions = new List<Permission>
        {
            new Permission("ScreenManagement", "Manage screens"),
            new Permission("CampaignManagement", "Manage campaigns"),
            new Permission("UserManagement", "Manage users"),
            new Permission("BookingManagement", "Manage bookings"),
            new Permission("FinancialManagement", "Manage financials"),
            new Permission("Analytics", "View analytics"),
            new Permission("Settings", "Manage settings")
        };

        return _mapper.Map<IReadOnlyList<PermissionDto>>(permissions);
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
