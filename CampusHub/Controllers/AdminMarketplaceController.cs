using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using FluentResults.Extensions.AspNetCore;
using CampusHub.Application.DTO.Marketplace;
using CampusHub.Application.DTO.Admin;

namespace CampusHub.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/marketplace")]
    public class AdminMarketplaceController : Controller
    {
        private readonly IAdminMarketplaceService _adminMarketplaceService;

        public AdminMarketplaceController(IAdminMarketplaceService adminMarketplaceService)
        {
            _adminMarketplaceService = adminMarketplaceService;
        }

        [HttpGet("items")]
        public async Task<ActionResult<List<MarketplaceItemDto>>> GetAllItems()
        {
            var result = await _adminMarketplaceService.GetAllItemsAsync();
            return result.ToActionResult();
        }

        [HttpGet("reports")]
        public async Task<ActionResult<List<ReportDto>>> GetAllReports()
        {
            var result = await _adminMarketplaceService.GetAllReportsAsync();
            return result.ToActionResult();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<AdminMarketplaceStatsDto>> GetMarketplaceStats()
        {
            var result = await _adminMarketplaceService.GetMarketplaceStatsAsync();
            return result.ToActionResult();
        }

        [HttpPut("items/{itemId}/status")]
        public async Task<ActionResult> UpdateItemStatus(int itemId, [FromBody] UpdateItemStatusRequest request)
        {
            var result = await _adminMarketplaceService.UpdateItemStatusAsync(itemId, request.Status);
            return result.ToActionResult();
        }

        [HttpPut("reports/{reportId}/status")]
        public async Task<ActionResult> UpdateReportStatus(int reportId, [FromBody] UpdateReportStatusDto request)
        {
            var result = await _adminMarketplaceService.UpdateReportStatusAsync(
                reportId,
                request.Status,
                request.AdminUserId,
                request.AdminNotes);

            return result.ToActionResult();
        }

        [HttpDelete("items/{itemId}")]
        public async Task<ActionResult> DeleteItem(int itemId)
        {
            var result = await _adminMarketplaceService.DeleteItemAsync(itemId);
            return result.ToActionResult();
        }
    }

    public class UpdateItemStatusRequest
    {
        public MarketplaceItemStatus Status { get; set; }
    }
}
