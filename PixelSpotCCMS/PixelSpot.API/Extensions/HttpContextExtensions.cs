using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace PixelSpot.API.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// Safely extracts the authentication token from the Authorization header
    /// </summary>
    /// <param name="context">The HttpContext</param>
    /// <returns>The token or an empty string if not found</returns>
    public static string GetAuthToken(this HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return string.Empty;
        }

        return authHeader.Substring("Bearer ".Length).Trim();
    }

    /// <summary>
    /// Gets the current user's ID from the JWT claims
    /// </summary>
    /// <param name="context">The HttpContext</param>
    /// <returns>The user ID as Guid or Guid.Empty if not found</returns>
    public static Guid GetCurrentUserId(this HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            return Guid.Empty;
        }

        if (Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            return userId;
        }

        return Guid.Empty;
    }

    /// <summary>
    /// Gets the current user's role from the JWT claims
    /// </summary>
    /// <param name="context">The HttpContext</param>
    /// <returns>The user role or an empty string if not found</returns>
    public static string GetCurrentUserRole(this HttpContext context)
    {
        var roleClaim = context.User.FindFirst(ClaimTypes.Role);
        return roleClaim?.Value ?? string.Empty;
    }
}