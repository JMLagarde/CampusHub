using CampusHub.Domain.Entities;
using FluentResults;

namespace CampusHub.Application.Interfaces
{
    public interface IAdminUserRepository
    {
        Task<Result<List<User>>> GetAllStudentsAsync();
        Task<Result<User>> GetStudentByIdAsync(int id);
        Task<Result> BanUserAsync(int userId);
        Task<Result> UnbanUserAsync(int userId);
        Task<Result> ResetPasswordAsync(int userId);
        Task<Result<int>> GetTotalStudentsCountAsync();
        Task<Result<int>> GetActiveStudentsCountAsync();
        Task<Result<int>> GetBannedStudentsCountAsync();
        Task<Result<int>> GetReportsCountForUserAsync(int userId);
    }
}
