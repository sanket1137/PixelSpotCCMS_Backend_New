using PixelSpot.Application.DTOs;

namespace PixelSpot.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(Guid id);
    Task<IReadOnlyList<UserDto>> GetAllAsync();
    Task<IReadOnlyList<UserDto>> GetUsersByRoleAsync(string role);
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, string createdBy);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto, string modifiedBy);
    Task<UserDto> UpdateBankDetailsAsync(Guid id, UpdateBankDetailsDto bankDetailsDto, string modifiedBy);
    Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto changePasswordDto);
    Task<bool> SetUserActiveStatusAsync(Guid id, bool isActive, string modifiedBy);
    Task<bool> SetUserVerificationStatusAsync(Guid id, bool isVerified, string modifiedBy);
    Task<bool> DeleteUserAsync(Guid id);
    Task<IReadOnlyList<SubUserDto>> GetSubUsersByUserIdAsync(Guid userId);
    Task<SubUserDto> GetSubUserByIdAsync(Guid id);
    Task<SubUserDto> CreateSubUserAsync(Guid userId, CreateSubUserDto createSubUserDto, string createdBy);
    Task<SubUserDto> UpdateSubUserAsync(Guid id, UpdateSubUserDto updateSubUserDto, string modifiedBy);
    Task<bool> DeleteSubUserAsync(Guid id);
    Task<IReadOnlyList<PermissionDto>> GetAllPermissionsAsync();
}
