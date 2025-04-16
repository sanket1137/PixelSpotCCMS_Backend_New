using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelSpot.API.DTOs;
using PixelSpot.API.Extensions;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;

namespace PixelSpot.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly new IAuthService _authService; // Using 'new' to suppress hiding warning
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        IAuthService authService,
        ILogger<UsersController> logger)
        : base(authService)
    {
        _userService = userService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        return Ok(new ApiResponse<IEnumerable<UserDto>>(users));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            var userId = await GetCurrentUserId();
            var userRole = await GetCurrentUserRole();

            // Check if user is requesting their own data or is an admin
            if (id != userId && userRole != "Admin")
            {
                return Forbid();
            }

            var user = await _userService.GetByIdAsync(id);
            return Ok(new ApiResponse<UserDto>(user));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpGet("by-role/{role}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersByRole(string role)
    {
        var users = await _userService.GetUsersByRoleAsync(role);
        return Ok(new ApiResponse<IEnumerable<UserDto>>(users));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            var currentUserId = await GetCurrentUserId();
            var user = await _userService.CreateUserAsync(createUserDto, currentUserId.ToString());
            return Created($"/api/users/{user.Id}", new ApiResponse<UserDto>(user));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            var userId = await GetCurrentUserId();
            var userRole = await GetCurrentUserRole();

            // Check if user is updating their own data or is an admin
            if (id != userId && userRole != "Admin")
            {
                return Forbid();
            }

            var user = await _userService.UpdateUserAsync(id, updateUserDto, userId.ToString());
            return Ok(new ApiResponse<UserDto>(user));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(ex.Message));
        }
    }

    [HttpPut("{id}/bank-details")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBankDetails(Guid id, [FromBody] UpdateBankDetailsDto bankDetailsDto)
    {
        try
        {
            var userId = await GetCurrentUserId();
            var userRole = await GetCurrentUserRole();

            // Check if user is updating their own data or is an admin
            if (id != userId && userRole != "Admin")
            {
                return Forbid();
            }

            var user = await _userService.UpdateBankDetailsAsync(id, bankDetailsDto, userId.ToString());
            return Ok(new ApiResponse<UserDto>(user));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(ex.Message));
        }
    }

    [HttpPut("{id}/change-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var userId = await GetCurrentUserId();
            var userRole = await GetCurrentUserRole();

            // Check if user is changing their own password or is an admin
            if (id != userId && userRole != "Admin")
            {
                return Forbid();
            }

            await _userService.ChangePasswordAsync(id, changePasswordDto);
            return Ok(new ApiResponse("Password changed successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse(ex.Message));
        }
    }

    [HttpPut("{id}/active-status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetActiveStatus(Guid id, [FromQuery] bool isActive)
    {
        try
        {
            var userId = await GetCurrentUserId();
            await _userService.SetUserActiveStatusAsync(id, isActive, userId.ToString());
            return Ok(new ApiResponse($"User active status set to {isActive}."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpPut("{id}/verification-status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetVerificationStatus(Guid id, [FromQuery] bool isVerified)
    {
        try
        {
            var userId = await GetCurrentUserId();
            await _userService.SetUserVerificationStatusAsync(id, isVerified, userId.ToString());
            return Ok(new ApiResponse($"User verification status set to {isVerified}."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new ApiResponse("User deleted successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    // Using GetCurrentUserId and GetCurrentUserRole from BaseApiController
}
