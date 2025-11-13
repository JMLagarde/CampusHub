using CampusHub.Application.DTO.Admin;
using FluentResults;

namespace CampusHub.Application.Interfaces
{
    public interface IAdminUserService
    {
        Task<Result<List<AdminUserDto>>> GetAllStudentsAsync();
        Task<Result<AdminUserStatsDto>> GetUserStatsAsync();
        Task<Result> BanUserAsync(int userId);
        Task<Result> UnbanUserAsync(int userId);
        Task<Result> ResetPasswordAsync(int userId);
    }
}
