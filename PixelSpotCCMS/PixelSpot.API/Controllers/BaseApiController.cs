using Microsoft.AspNetCore.Mvc;
using PixelSpot.API.Extensions;
using PixelSpot.Application.Interfaces;

namespace PixelSpot.API.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected readonly IAuthService _authService;
    
    protected BaseApiController(IAuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }
    
    /// <summary>
    /// Gets the current user's ID from the JWT token
    /// </summary>
    /// <returns>The current user's ID or Guid.Empty if not found</returns>
    protected async Task<Guid> GetCurrentUserId()
    {
        // Use the extension method to get a non-null token
        var authToken = HttpContext.GetAuthToken();
        if (!string.IsNullOrEmpty(authToken))
        {
            return await _authService.GetUserIdFromTokenAsync(authToken);
        }
        return Guid.Empty;
    }

    /// <summary>
    /// Gets the current user's role from the JWT token
    /// </summary>
    /// <returns>The current user's role or an empty string if not found</returns>
    protected async Task<string> GetCurrentUserRole()
    {
        // Use the extension method to get a non-null token
        var authToken = HttpContext.GetAuthToken();
        if (!string.IsNullOrEmpty(authToken))
        {
            return await _authService.GetUserRoleFromTokenAsync(authToken);
        }
        return string.Empty;
    }
}