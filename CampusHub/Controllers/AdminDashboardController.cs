using CampusHub.Application.DTO.Admin;
using CampusHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusHub.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardService;

        public AdminDashboardController(IAdminDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await _dashboardService.GetDashboardStatsAsync();

            if (result.IsFailed)
            {
                return BadRequest(new { message = result.Errors.First().Message });
            }

            return Ok(result.Value);
        }
    }
}
