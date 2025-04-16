using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelSpot.API.DTOs;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;

namespace PixelSpot.API.Controllers;

[ApiController]
[Route("api/creatives")]
[Authorize]
public class CreativesController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    private readonly IAuthService _authService;
    private readonly ILogger<CreativesController> _logger;

    public CreativesController(
        ICampaignService campaignService,
        IAuthService authService,
        ILogger<CreativesController> logger)
    {
        _campaignService = campaignService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("by-campaign/{campaignId}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CreativeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCreativesByCampaignId(Guid campaignId)
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

            var creatives = await _campaignService.GetCampaignCreativesAsync(campaignId);
            return Ok(new ApiResponse<IEnumerable<CreativeDto>>(creatives));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Advertiser,Admin")]
    [ProducesResponseType(typeof(ApiResponse<CreativeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCreative([FromBody] CreateCreativeDto createCreativeDto)
    {
        try
        {
            var campaign = await _campaignService.GetByIdAsync(createCreativeDto.CampaignId);
            var currentUserId = await GetCurrentUserId();
            var currentUserRole = await GetCurrentUserRole();

            // Check if current user is the advertiser of the campaign or is an admin
            if (campaign.AdvertiserId != currentUserId && currentUserRole != "Admin")
            {
                return Forbid();
            }

            var creative = await _campaignService.AddCreativeAsync(createCreativeDto, currentUserId.ToString());
            return Created($"/api/creatives/{creative.Id}", new ApiResponse<CreativeDto>(creative));
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

    [HttpPut("{id}")]
    [Authorize(Roles = "Advertiser,Admin")]
    [ProducesResponseType(typeof(ApiResponse<CreativeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCreative(Guid id, [FromBody] UpdateCreativeDto updateCreativeDto)
    {
        try
        {
            // In a real application, we would check if the current user owns the creative
            var currentUserId = await GetCurrentUserId();
            var creative = await _campaignService.UpdateCreativeAsync(id, updateCreativeDto, currentUserId.ToString());
            return Ok(new ApiResponse<CreativeDto>(creative));
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
    [ProducesResponseType(typeof(ApiResponse<CreativeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveCreative(Guid id, [FromBody] ApproveRejectCreativeDto approveRejectDto)
    {
        try
        {
            var currentUserId = await GetCurrentUserId();
            var creative = await _campaignService.ApproveRejectCreativeAsync(id, approveRejectDto, currentUserId.ToString());
            return Ok(new ApiResponse<CreativeDto>(creative));
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
    public async Task<IActionResult> DeleteCreative(Guid id)
    {
        try
        {
            // In a real application, we would check if the current user owns the creative
            await _campaignService.DeleteCreativeAsync(id);
            return Ok(new ApiResponse("Creative deleted successfully."));
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
