using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelSpot.API.DTOs;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;

namespace PixelSpot.API.Controllers;

[ApiController]
[Route("api/users/{userId}/sub-users")]
[Authorize]
public class SubUsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly ILogger<SubUsersController> _logger;

    public SubUsersController(
        IUserService userService,
        IAuthService authService,
        ILogger<SubUsersController> logger)
    {
        _userService = userService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubUserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSubUsers(Guid userId)
    {
        var currentUserId = await GetCurrentUserId();
        var currentUserRole = await GetCurrentUserRole();

        // Check if current user is requesting their own sub-users or is an admin
        if (userId != currentUserId && currentUserRole != "Admin")
        {
            return Forbid();
        }

        var subUsers = await _userService.GetSubUsersByUserIdAsync(userId);
        return Ok(new ApiResponse<IEnumerable<SubUserDto>>(subUsers));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SubUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSubUserById(Guid userId, Guid id)
    {
        try
        {
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is requesting their own sub-user or is an admin
            if (userId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var subUser = await _userService.GetSubUserByIdAsync(id);
            
            // Verify this sub-user belongs to the specified user
            if (subUser.UserId != userId)
            {
                return NotFound(new ApiResponse("Sub-user not found for this user."));
            }
            
            return Ok(new ApiResponse<SubUserDto>(subUser));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SubUserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateSubUser(Guid userId, [FromBody] CreateSubUserDto createSubUserDto)
    {
        try
        {
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is creating their own sub-user or is an admin
            if (userId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var subUser = await _userService.CreateSubUserAsync(userId, createSubUserDto, currentUserId.ToString());
            return Created($"/api/users/{userId}/sub-users/{subUser.Id}", new ApiResponse<SubUserDto>(subUser));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SubUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateSubUser(Guid userId, Guid id, [FromBody] UpdateSubUserDto updateSubUserDto)
    {
        try
        {
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is updating their own sub-user or is an admin
            if (userId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var subUser = await _userService.GetSubUserByIdAsync(id);
            
            // Verify this sub-user belongs to the specified user
            if (subUser.UserId != userId)
            {
                return NotFound(new ApiResponse("Sub-user not found for this user."));
            }
            
            var updatedSubUser = await _userService.UpdateSubUserAsync(id, updateSubUserDto, currentUserId.ToString());
            return Ok(new ApiResponse<SubUserDto>(updatedSubUser));
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

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteSubUser(Guid userId, Guid id)
    {
        try
        {
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is deleting their own sub-user or is an admin
            if (userId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var subUser = await _userService.GetSubUserByIdAsync(id);
            
            // Verify this sub-user belongs to the specified user
            if (subUser.UserId != userId)
            {
                return NotFound(new ApiResponse("Sub-user not found for this user."));
            }
            
            await _userService.DeleteSubUserAsync(id);
            return Ok(new ApiResponse("Sub-user deleted successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpGet("permissions")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPermissions()
    {
        var permissions = await _userService.GetAllPermissionsAsync();
        return Ok(new ApiResponse<IEnumerable<PermissionDto>>(permissions));
    }

    private async Task<Guid> GetCurrentUserId()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        return await _authService.GetUserIdFromTokenAsync(authHeader);
    }

    private async Task<string> GetCurrentUserRole()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        return await _authService.GetUserRoleFromTokenAsync(authHeader);
    }
}
