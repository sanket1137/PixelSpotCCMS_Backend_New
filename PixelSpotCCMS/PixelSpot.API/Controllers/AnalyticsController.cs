using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelSpot.API.DTOs;
using PixelSpot.API.Extensions;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;

namespace PixelSpot.API.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : BaseApiController
{
    private readonly IScreenService _screenService;
    private readonly ICampaignService _campaignService;
    private readonly new IAuthService _authService; // Using 'new' to suppress hiding warning
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IScreenService screenService,
        ICampaignService campaignService,
        IAuthService authService,
        ILogger<AnalyticsController> logger)
        : base(authService)
    {
        _screenService = screenService;
        _campaignService = campaignService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("screens/{screenId}/metrics")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ScreenMetricsDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetScreenMetrics(Guid screenId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var screen = await _screenService.GetByIdAsync(screenId);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the owner of the screen or is an admin
            if (screen.OwnerId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var metrics = await _screenService.GetScreenMetricsAsync(screenId, startDate, endDate);
            return Ok(new ApiResponse<IEnumerable<ScreenMetricsDto>>(metrics));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpGet("campaigns/{campaignId}/bookings")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ScreenBookingDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCampaignBookings(Guid campaignId)
    {
        try
        {
            var campaign = await _campaignService.GetByIdAsync(campaignId);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the advertiser of the campaign or is an admin
            if (campaign.AdvertiserId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var bookings = await _campaignService.GetCampaignBookingsAsync(campaignId);
            return Ok(new ApiResponse<IEnumerable<ScreenBookingDto>>(bookings));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpGet("campaigns/{campaignId}/summary")]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCampaignSummary(Guid campaignId)
    {
        try
        {
            var campaign = await _campaignService.GetByIdAsync(campaignId);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the advertiser of the campaign or is an admin
            if (campaign.AdvertiserId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            return Ok(new ApiResponse<CampaignDto>(campaign));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpGet("screens/{screenId}/bookings")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ScreenBookingDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetScreenBookings(Guid screenId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            var screen = await _screenService.GetByIdAsync(screenId);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the owner of the screen or is an admin
            if (screen.OwnerId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var bookings = await _screenService.GetScreenBookingsAsync(screenId, startDate, endDate);
            return Ok(new ApiResponse<IEnumerable<ScreenBookingDto>>(bookings));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    // Using GetCurrentUserId and GetCurrentUserRole from BaseApiController
}
