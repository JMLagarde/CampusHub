using CampusHub.Application.DTO.Admin;
using FluentResults;

namespace CampusHub.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<Result<AdminDashboardStatsDto>> GetDashboardStatsAsync();
    }
}
