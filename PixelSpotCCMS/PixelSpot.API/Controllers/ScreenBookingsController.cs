using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelSpot.API.DTOs;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;

namespace PixelSpot.API.Controllers;

[ApiController]
[Route("api/screen-bookings")]
[Authorize]
public class ScreenBookingsController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    private readonly IScreenService _screenService;
    private readonly IAuthService _authService;
    private readonly ILogger<ScreenBookingsController> _logger;

    public ScreenBookingsController(
        ICampaignService campaignService,
        IScreenService screenService,
        IAuthService authService,
        ILogger<ScreenBookingsController> logger)
    {
        _campaignService = campaignService;
        _screenService = screenService;
        _authService = authService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Advertiser,Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScreenBookingDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateBooking([FromBody] CreateScreenBookingDto createBookingDto)
    {
        try
        {
            var campaign = await _campaignService.GetByIdAsync(createBookingDto.CampaignId);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the advertiser of the campaign or is an admin
            if (campaign.AdvertiserId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            // Check if the screen is available
            var isAvailable = await _screenService.IsScreenAvailableAsync(
                createBookingDto.ScreenId, 
                createBookingDto.StartTime, 
                createBookingDto.EndTime);

            if (!isAvailable)
            {
                return BadRequest(new ApiResponse("The screen is not available during the specified time period."));
            }

            var booking = await _campaignService.CreateBookingAsync(createBookingDto, currentUserId.ToString());
            return Created($"/api/screen-bookings/{booking.Id}", new ApiResponse<ScreenBookingDto>(booking));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(ex.Message));
        }
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "ScreenOwner,Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScreenBookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBookingStatus(Guid id, [FromBody] UpdateScreenBookingStatusDto statusDto)
    {
        try
        {
            // In a real application, we would check if the current user is the screen owner
            var currentUserId = await GetCurrentUserId();
            var booking = await _campaignService.UpdateBookingStatusAsync(id, statusDto, currentUserId.ToString());
            return Ok(new ApiResponse<ScreenBookingDto>(booking));
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

    [HttpPut("{id}/payment")]
    [Authorize(Roles = "Advertiser,Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScreenBookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBookingPayment(Guid id, [FromBody] UpdateScreenBookingPaymentDto paymentDto)
    {
        try
        {
            // In a real application, we would check if the current user is the advertiser
            var currentUserId = await GetCurrentUserId();
            var booking = await _campaignService.UpdateBookingPaymentAsync(id, paymentDto, currentUserId.ToString());
            return Ok(new ApiResponse<ScreenBookingDto>(booking));
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
