using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelSpot.API.DTOs;
using PixelSpot.API.Extensions;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;

namespace PixelSpot.API.Controllers;

[ApiController]
[Route("api/campaigns")]
[Authorize]
public class CampaignsController : BaseApiController
{
    private readonly ICampaignService _campaignService;
    private readonly new IAuthService _authService; // Using 'new' to suppress hiding warning
    private readonly ILogger<CampaignsController> _logger;

    public CampaignsController(
        ICampaignService campaignService,
        IAuthService authService,
        ILogger<CampaignsController> logger)
        : base(authService)
    {
        _campaignService = campaignService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CampaignDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCampaigns()
    {
        var campaigns = await _campaignService.GetAllAsync();
        return Ok(new ApiResponse<IEnumerable<CampaignDto>>(campaigns));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCampaignById(Guid id)
    {
        try
        {
            var campaign = await _campaignService.GetByIdAsync(id);
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

    [HttpGet("by-advertiser/{advertiserId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CampaignDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCampaignsByAdvertiserId(Guid advertiserId)
    {
        var currentUserId = await GetCurrentUserId();
        var currentUserRole = await GetCurrentUserRole();

        // Check if current user is requesting their own campaigns or is an admin
        if (advertiserId != currentUserId && currentUserRole != "Admin")
        {
            return Forbid();
        }

        var campaigns = await _campaignService.GetCampaignsByAdvertiserIdAsync(advertiserId);
        return Ok(new ApiResponse<IEnumerable<CampaignDto>>(campaigns));
    }

    [HttpPost]
    [Authorize(Roles = "Advertiser,Admin")]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDto createCampaignDto)
    {
        try
        {
            var currentUserId = await GetCurrentUserId();
            var campaign = await _campaignService.CreateCampaignAsync(currentUserId, createCampaignDto, currentUserId.ToString());
            return Created($"/api/campaigns/{campaign.Id}", new ApiResponse<CampaignDto>(campaign));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Advertiser,Admin")]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCampaign(Guid id, [FromBody] UpdateCampaignDto updateCampaignDto)
    {
        try
        {
            var campaign = await _campaignService.GetByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the advertiser of the campaign or is an admin
            if (campaign.AdvertiserId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var updatedCampaign = await _campaignService.UpdateCampaignAsync(id, updateCampaignDto, currentUserId.ToString());
            return Ok(new ApiResponse<CampaignDto>(updatedCampaign));
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
    [Authorize(Roles = "Advertiser,Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteCampaign(Guid id)
    {
        try
        {
            var campaign = await _campaignService.GetByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the advertiser of the campaign or is an admin
            if (campaign.AdvertiserId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            await _campaignService.DeleteCampaignAsync(id);
            return Ok(new ApiResponse("Campaign deleted successfully."));
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
    public async Task<IActionResult> GetCampaignBookings(Guid id)
    {
        try
        {
            var campaign = await _campaignService.GetByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the advertiser of the campaign or is an admin
            if (campaign.AdvertiserId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var bookings = await _campaignService.GetCampaignBookingsAsync(id);
            return Ok(new ApiResponse<IEnumerable<ScreenBookingDto>>(bookings));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    // Using GetCurrentUserId and GetCurrentUserRole from BaseApiController
}
