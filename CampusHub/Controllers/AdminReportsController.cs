using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using FluentResults.Extensions.AspNetCore;
using CampusHub.Application.DTO.Marketplace;
using CampusHub.Application.DTO.Admin;

namespace CampusHub.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/reports")]
    public class AdminReportsController : Controller
    {
        private readonly IAdminReportsService _adminReportsService;

        public AdminReportsController(IAdminReportsService adminReportsService)
        {
            _adminReportsService = adminReportsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReportDto>>> GetAllReports()
        {
            var result = await _adminReportsService.GetAllReportsAsync();
            return result.ToActionResult();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<AdminReportsStatsDto>> GetReportsStats()
        {
            var result = await _adminReportsService.GetReportsStatsAsync();
            return result.ToActionResult();
        }

        [HttpPut("{reportId}/status")]
        public async Task<ActionResult> UpdateReportStatus(int reportId, [FromBody] UpdateReportStatusDto request)
        {
            var result = await _adminReportsService.UpdateReportStatusAsync(
                reportId,
                request.Status,
                request.AdminUserId,
                request.AdminNotes);

            return result.ToActionResult();
        }

        [HttpPut("items/{itemId}/flag")]
        public async Task<ActionResult> FlagItem(int itemId, [FromBody] FlagItemRequest request)
        {
            var result = await _adminReportsService.FlagItemAsync(itemId, request.AdminUserId);
            return result.ToActionResult();
        }
    }

    public class FlagItemRequest
    {
        public int AdminUserId { get; set; }
    }
}
