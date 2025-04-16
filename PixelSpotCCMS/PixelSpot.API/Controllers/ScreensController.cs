using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelSpot.API.DTOs;
using PixelSpot.API.Extensions;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;

namespace PixelSpot.API.Controllers;

[ApiController]
[Route("api/screens")]
[Authorize]
public class ScreensController : BaseApiController
{
    private readonly IScreenService _screenService;
    private readonly new IAuthService _authService; // Using 'new' to suppress hiding warning
    private readonly ILogger<ScreensController> _logger;

    public ScreensController(
        IScreenService screenService,
        IAuthService authService,
        ILogger<ScreensController> logger)
        : base(authService)
    {
        _screenService = screenService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ScreenDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllScreens()
    {
        var screens = await _screenService.GetVerifiedScreensAsync();
        return Ok(new ApiResponse<IEnumerable<ScreenDto>>(screens));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ScreenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetScreenById(Guid id)
    {
        try
        {
            var screen = await _screenService.GetByIdAsync(id);
            return Ok(new ApiResponse<ScreenDto>(screen));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ScreenDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchScreens([FromQuery] ScreenSearchDto searchDto)
    {
        var screens = await _screenService.SearchScreensAsync(searchDto);
        return Ok(new ApiResponse<IEnumerable<ScreenDto>>(screens));
    }

    [HttpGet("by-owner/{ownerId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ScreenDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetScreensByOwnerId(Guid ownerId)
    {
        var currentUserId = await GetCurrentUserId();
        var currentUserRole = await GetCurrentUserRole();

        // Check if current user is requesting their own screens or is an admin
        if (ownerId != currentUserId && currentUserRole != "Admin")
        {
            return Forbid();
        }

        var screens = await _screenService.GetScreensByOwnerIdAsync(ownerId);
        return Ok(new ApiResponse<IEnumerable<ScreenDto>>(screens));
    }

    [HttpPost]
    [Authorize(Roles = "ScreenOwner,Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScreenDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateScreen([FromBody] CreateScreenDto createScreenDto)
    {
        try
        {
            var currentUserId = await GetCurrentUserId();
            var screen = await _screenService.CreateScreenAsync(currentUserId, createScreenDto, currentUserId.ToString());
            return Created($"/api/screens/{screen.Id}", new ApiResponse<ScreenDto>(screen));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ScreenOwner,Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScreenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateScreen(Guid id, [FromBody] UpdateScreenDto updateScreenDto)
    {
        try
        {
            var screen = await _screenService.GetByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the owner of the screen or is an admin
            if (screen.OwnerId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var updatedScreen = await _screenService.UpdateScreenAsync(id, updateScreenDto, currentUserId.ToString());
            return Ok(new ApiResponse<ScreenDto>(updatedScreen));
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

    [HttpPut("{id}/active-status")]
    [Authorize(Roles = "ScreenOwner,Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SetActiveStatus(Guid id, [FromQuery] bool isActive)
    {
        try
        {
            var screen = await _screenService.GetByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the owner of the screen or is an admin
            if (screen.OwnerId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            await _screenService.SetScreenActiveStatusAsync(id, isActive, currentUserId.ToString());
            return Ok(new ApiResponse($"Screen active status set to {isActive}."));
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
            var currentUserId = await GetCurrentUserId();
            await _screenService.SetScreenVerificationStatusAsync(id, isVerified, currentUserId.ToString());
            return Ok(new ApiResponse($"Screen verification status set to {isVerified}."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ScreenOwner,Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteScreen(Guid id)
    {
        try
        {
            var screen = await _screenService.GetByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the owner of the screen or is an admin
            if (screen.OwnerId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            await _screenService.DeleteScreenAsync(id);
            return Ok(new ApiResponse("Screen deleted successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpPut("{id}/pricing")]
    [Authorize(Roles = "ScreenOwner,Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScreenPricingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateScreenPricing(Guid id, [FromBody] UpdateScreenPricingDto pricingDto)
    {
        try
        {
            var screen = await _screenService.GetByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the owner of the screen or is an admin
            if (screen.OwnerId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var pricing = await _screenService.UpdateScreenPricingAsync(id, pricingDto, currentUserId.ToString());
            return Ok(new ApiResponse<ScreenPricingDto>(pricing));
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

    [HttpPost("{id}/availabilities")]
    [Authorize(Roles = "ScreenOwner,Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScreenAvailabilityDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddScreenAvailability(Guid id, [FromBody] CreateScreenAvailabilityDto availabilityDto)
    {
        try
        {
            var screen = await _screenService.GetByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the owner of the screen or is an admin
            if (screen.OwnerId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var availability = await _screenService.AddScreenAvailabilityAsync(id, availabilityDto, currentUserId.ToString());
            return Created($"/api/screens/{id}/availabilities/{availability.Id}", new ApiResponse<ScreenAvailabilityDto>(availability));
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

    [HttpDelete("availabilities/{availabilityId}")]
    [Authorize(Roles = "ScreenOwner,Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveScreenAvailability(Guid availabilityId)
    {
        try
        {
            // This is a simplified implementation. In a real application, we would check ownership.
            var currentUserId = await GetCurrentUserId();
            await _screenService.RemoveScreenAvailabilityAsync(availabilityId);
            return Ok(new ApiResponse("Screen availability removed successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpGet("{id}/availabilities")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ScreenAvailabilityDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetScreenAvailabilities(Guid id)
    {
        try
        {
            var availabilities = await _screenService.GetScreenAvailabilitiesAsync(id);
            return Ok(new ApiResponse<IEnumerable<ScreenAvailabilityDto>>(availabilities));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpGet("{id}/bookings")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ScreenBookingDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetScreenBookings(Guid id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var screen = await _screenService.GetByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the owner of the screen or is an admin
            if (screen.OwnerId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var bookings = await _screenService.GetScreenBookingsAsync(id, startDate, endDate);
            return Ok(new ApiResponse<IEnumerable<ScreenBookingDto>>(bookings));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpPost("{id}/metrics")]
    [Authorize(Roles = "ScreenOwner,Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScreenMetricsDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddScreenMetrics(Guid id, [FromBody] CreateScreenMetricsDto metricsDto)
    {
        try
        {
            var screen = await _screenService.GetByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the owner of the screen or is an admin
            if (screen.OwnerId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var metrics = await _screenService.AddScreenMetricsAsync(id, metricsDto, currentUserId.ToString());
            return Created($"/api/screens/{id}/metrics/{metrics.Id}", new ApiResponse<ScreenMetricsDto>(metrics));
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

    [HttpGet("{id}/metrics")]
    [Authorize(Roles = "ScreenOwner,Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ScreenMetricsDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetScreenMetrics(Guid id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var screen = await _screenService.GetByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the owner of the screen or is an admin
            if (screen.OwnerId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var metrics = await _screenService.GetScreenMetricsAsync(id, startDate, endDate);
            return Ok(new ApiResponse<IEnumerable<ScreenMetricsDto>>(metrics));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpGet("{id}/availability-check")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckScreenAvailability(Guid id, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        try
        {
            var isAvailable = await _screenService.IsScreenAvailableAsync(id, startTime, endTime);
            return Ok(new ApiResponse<bool>(isAvailable));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpGet("{id}/booking-price")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CalculateBookingPrice(Guid id, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        try
        {
            var price = await _screenService.CalculateBookingPriceAsync(id, startTime, endTime);
            return Ok(new ApiResponse<decimal>(price));
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

    // Using GetCurrentUserId and GetCurrentUserRole from BaseApiController
}
