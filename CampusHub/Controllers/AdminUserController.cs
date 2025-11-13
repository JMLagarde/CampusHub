using CampusHub.Application.DTO.Admin;
using CampusHub.Application.Interfaces;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace CampusHub.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    public class AdminUserController : Controller
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AdminUserDto>>> GetAllStudents()
        {
            var result = await _adminUserService.GetAllStudentsAsync();
            return result.ToActionResult();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<AdminUserStatsDto>> GetUserStats()
        {
            var result = await _adminUserService.GetUserStatsAsync();
            return result.ToActionResult();
        }

        [HttpPut("{userId}/ban")]
        public async Task<ActionResult> BanUser(int userId)
        {
            var result = await _adminUserService.BanUserAsync(userId);
            return result.ToActionResult();
        }

        [HttpPut("{userId}/unban")]
        public async Task<ActionResult> UnbanUser(int userId)
        {
            var result = await _adminUserService.UnbanUserAsync(userId);
            return result.ToActionResult();
        }

        [HttpPut("{userId}/reset-password")]
        public async Task<ActionResult> ResetPassword(int userId)
        {
            var result = await _adminUserService.ResetPasswordAsync(userId);
            return result.ToActionResult();
        }
    }
}
