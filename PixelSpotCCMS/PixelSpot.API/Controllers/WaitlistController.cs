using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelSpot.API.DTOs;
using PixelSpot.API.Extensions;
using PixelSpot.Application.DTOs;
using PixelSpot.Application.Interfaces;

namespace PixelSpot.API.Controllers;

[ApiController]
[Route("api/waitlist")]
public class WaitlistController : BaseApiController
{
    private readonly ICampaignService _campaignService;
    private readonly new IAuthService _authService; // Using 'new' to suppress hiding warning
    private readonly ILogger<WaitlistController> _logger;

    public WaitlistController(
        ICampaignService campaignService,
        IAuthService authService,
        ILogger<WaitlistController> logger)
        : base(authService)
    {
        _campaignService = campaignService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WaitlistEntryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllWaitlistEntries()
    {
        var entries = await _campaignService.GetAllWaitlistEntriesAsync();
        return Ok(new ApiResponse<IEnumerable<WaitlistEntryDto>>(entries));
    }

    [HttpGet("by-status/{status}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<WaitlistEntryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWaitlistEntriesByStatus(string status)
    {
        var entries = await _campaignService.GetWaitlistEntriesByStatusAsync(status);
        return Ok(new ApiResponse<IEnumerable<WaitlistEntryDto>>(entries));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<WaitlistEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWaitlistEntryById(Guid id)
    {
        try
        {
            var entry = await _campaignService.GetWaitlistEntryByIdAsync(id);
            return Ok(new ApiResponse<WaitlistEntryDto>(entry));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<WaitlistEntryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWaitlistEntry([FromBody] CreateWaitlistEntryDto createEntryDto)
    {
        try
        {
            var entry = await _campaignService.CreateWaitlistEntryAsync(createEntryDto);
            return Created($"/api/waitlist/{entry.Id}", new ApiResponse<WaitlistEntryDto>(entry));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<WaitlistEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWaitlistEntry(Guid id, [FromBody] UpdateWaitlistEntryDto updateEntryDto)
    {
        try
        {
            var entry = await _campaignService.UpdateWaitlistEntryAsync(id, updateEntryDto);
            return Ok(new ApiResponse<WaitlistEntryDto>(entry));
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

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<WaitlistEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWaitlistEntryStatus(Guid id, [FromBody] UpdateWaitlistEntryStatusDto statusDto)
    {
        try
        {
            var currentUserId = await GetCurrentUserId();
            var entry = await _campaignService.UpdateWaitlistEntryStatusAsync(id, statusDto, currentUserId.ToString());
            return Ok(new ApiResponse<WaitlistEntryDto>(entry));
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
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWaitlistEntry(Guid id)
    {
        try
        {
            await _campaignService.DeleteWaitlistEntryAsync(id);
            return Ok(new ApiResponse("Waitlist entry deleted successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse(ex.Message));
        }
    }

    // Using GetCurrentUserId from BaseApiController
}
