using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelSpot.API.DTOs;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;

namespace PixelSpot.API.Controllers;

[ApiController]
[Route("api/campaign-requests")]
[Authorize]
public class CampaignRequestsController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    private readonly IAuthService _authService;
    private readonly ILogger<CampaignRequestsController> _logger;

    public CampaignRequestsController(
        ICampaignService campaignService,
        IAuthService authService,
        ILogger<CampaignRequestsController> logger)
    {
        _campaignService = campaignService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CampaignRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingCampaignRequests()
    {
        var requests = await _campaignService.GetPendingCampaignRequestsAsync();
        return Ok(new ApiResponse<IEnumerable<CampaignRequestDto>>(requests));
    }

    [HttpGet("by-advertiser/{advertiserId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CampaignRequestDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCampaignRequestsByAdvertiserId(Guid advertiserId)
    {
        var currentUserId = await GetCurrentUserId();
        var currentUserRole = await GetCurrentUserRole();

        // Check if current user is requesting their own campaign requests or is an admin
        if (advertiserId != currentUserId && currentUserRole != "Admin")
        {
            return Forbid();
        }

        var requests = await _campaignService.GetCampaignRequestsByAdvertiserIdAsync(advertiserId);
        return Ok(new ApiResponse<IEnumerable<CampaignRequestDto>>(requests));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CampaignRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCampaignRequestById(Guid id)
    {
        try
        {
            var request = await _campaignService.GetCampaignRequestByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the advertiser of the request or is an admin
            if (request.AdvertiserId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            return Ok(new ApiResponse<CampaignRequestDto>(request));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Advertiser")]
    [ProducesResponseType(typeof(ApiResponse<CampaignRequestDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCampaignRequest([FromBody] CreateCampaignRequestDto createRequestDto)
    {
        try
        {
            var currentUserId = await GetCurrentUserId();
            var request = await _campaignService.CreateCampaignRequestAsync(currentUserId, createRequestDto, currentUserId.ToString());
            return Created($"/api/campaign-requests/{request.Id}", new ApiResponse<CampaignRequestDto>(request));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Advertiser")]
    [ProducesResponseType(typeof(ApiResponse<CampaignRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCampaignRequest(Guid id, [FromBody] UpdateCampaignRequestDto updateRequestDto)
    {
        try
        {
            var request = await _campaignService.GetCampaignRequestByIdAsync(id);
            var currentUserId = await GetCurrentUserId();

            // Check if current user is the advertiser of the request
            if (request.AdvertiserId != currentUserId)
            {
                return Forbid();
            }

            // Check if request is in pending status
            if (request.Status != "Pending")
            {
                return BadRequest(new ApiResponse("Only pending requests can be updated."));
            }

            var updatedRequest = await _campaignService.UpdateCampaignRequestAsync(id, updateRequestDto, currentUserId.ToString());
            return Ok(new ApiResponse<CampaignRequestDto>(updatedRequest));
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

    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CampaignRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveCampaignRequest(Guid id, [FromBody] ApproveCampaignRequestDto approveDto)
    {
        try
        {
            var currentUserId = await GetCurrentUserId();
            var updatedRequest = await _campaignService.ApproveCampaignRequestAsync(id, approveDto, currentUserId.ToString());
            return Ok(new ApiResponse<CampaignRequestDto>(updatedRequest));
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

    [HttpDelete("{id}")]
    [Authorize(Roles = "Advertiser,Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteCampaignRequest(Guid id)
    {
        try
        {
            var request = await _campaignService.GetCampaignRequestByIdAsync(id);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the advertiser of the request or is an admin
            if (request.AdvertiserId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            await _campaignService.DeleteCampaignRequestAsync(id);
            return Ok(new ApiResponse("Campaign request deleted successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
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
