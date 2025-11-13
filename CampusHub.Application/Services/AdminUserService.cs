using CampusHub.Application.DTO.Admin;
using CampusHub.Application.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace CampusHub.Application.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly ILogger<AdminUserService> _logger;

        public AdminUserService(
            IAdminUserRepository adminUserRepository,
            ILogger<AdminUserService> logger)
        {
            _adminUserRepository = adminUserRepository;
            _logger = logger;
        }

        public async Task<Result<List<AdminUserDto>>> GetAllStudentsAsync()
        {
            try
            {
                var studentsResult = await _adminUserRepository.GetAllStudentsAsync();
                if (studentsResult.IsFailed)
                {
                    return Result.Fail(studentsResult.Errors);
                }

                var studentDtos = new List<AdminUserDto>();
                foreach (var student in studentsResult.Value)
                {
                    var reportsCountResult = await _adminUserRepository.GetReportsCountForUserAsync(student.UserID);
                    var reportsCount = reportsCountResult.IsSuccess ? reportsCountResult.Value : 0;

                    var dto = MapToAdminUserDto(student, reportsCount);
                    studentDtos.Add(dto);
                }

                return Result.Ok(studentDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all students for admin");
                return Result.Fail(new Error("An error occurred while retrieving students")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<AdminUserStatsDto>> GetUserStatsAsync()
        {
            try
            {
                var totalResult = await _adminUserRepository.GetTotalStudentsCountAsync();
                var activeResult = await _adminUserRepository.GetActiveStudentsCountAsync();
                var bannedResult = await _adminUserRepository.GetBannedStudentsCountAsync();

                if (totalResult.IsFailed || activeResult.IsFailed || bannedResult.IsFailed)
                {
                    return Result.Fail(new Error("Failed to retrieve user statistics"));
                }

                var stats = new AdminUserStatsDto
                {
                    TotalUsers = totalResult.Value,
                    ActiveUsers = activeResult.Value,
                    BannedUsers = bannedResult.Value
                };

                return Result.Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user statistics");
                return Result.Fail(new Error("An error occurred while retrieving user statistics")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> BanUserAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result.Fail(new Error("Invalid user ID"));
                }

                return await _adminUserRepository.BanUserAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error banning user {UserId}", userId);
                return Result.Fail(new Error("An error occurred while banning user")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> UnbanUserAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result.Fail(new Error("Invalid user ID"));
                }

                return await _adminUserRepository.UnbanUserAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unbanning user {UserId}", userId);
                return Result.Fail(new Error("An error occurred while unbanning user")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> ResetPasswordAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result.Fail(new Error("Invalid user ID"));
                }

                return await _adminUserRepository.ResetPasswordAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user {UserId}", userId);
                return Result.Fail(new Error("An error occurred while resetting password")
                    .CausedBy(ex));
            }
        }

        private static AdminUserDto MapToAdminUserDto(Domain.Entities.User user, int reportsCount)
        {
            return new AdminUserDto
            {
                UserID = user.UserID,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                ContactNumber = user.ContactNumber,
                StudentNumber = user.StudentNumber,
                Program = user.Program?.Name,
                Year = user.YearLevel?.Name,
                Role = user.Role,
                Status = user.Status,
                ReportsCount = reportsCount,
                ListingsCount = user.MarketplaceItems.Count,
                JoinDate = user.DateRegistered?.ToString("yyyy-MM-dd"),
                Verified = !string.IsNullOrEmpty(user.Email), // Assuming verified if email exists
                DateRegistered = user.DateRegistered
            };
        }
    }
}
