using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.Application.Repositories
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly IApplicationDbContext _context;

        public AdminUserRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<User>>> GetAllStudentsAsync()
        {
            try
            {
                var students = await _context.Users
                    .Where(u => u.Role == "Student")
                    .Include(u => u.Program)
                    .Include(u => u.YearLevel)
                    .Include(u => u.MarketplaceItems)
                    .AsNoTracking()
                    .ToListAsync();

                return Result.Ok(students);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to retrieve students")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<User>> GetStudentByIdAsync(int id)
        {
            try
            {
                var student = await _context.Users
                    .Where(u => u.Role == "Student" && u.UserID == id)
                    .FirstOrDefaultAsync();

                if (student == null)
                {
                    return Result.Fail(new Error("Student not found"));
                }

                return Result.Ok(student);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to retrieve student")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> BanUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return Result.Fail(new Error("User not found"));
                }

                user.Status = "Banned";
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to ban user")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> UnbanUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return Result.Fail(new Error("User not found"));
                }

                user.Status = "Active";
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to unban user")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> ResetPasswordAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return Result.Fail(new Error("User not found"));
                }

                user.Password = user.StudentNumber ?? string.Empty;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to reset password")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<int>> GetTotalStudentsCountAsync()
        {
            try
            {
                var count = await _context.Users
                    .CountAsync(u => u.Role == "Student");
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get total students count")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<int>> GetActiveStudentsCountAsync()
        {
            try
            {
                var count = await _context.Users
                    .CountAsync(u => u.Role == "Student" && u.Status == "Active");
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get active students count")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<int>> GetBannedStudentsCountAsync()
        {
            try
            {
                var count = await _context.Users
                    .CountAsync(u => u.Role == "Student" && u.Status == "Banned");
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get banned students count")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<int>> GetReportsCountForUserAsync(int userId)
        {
            try
            {
                var count = await _context.Reports
                    .CountAsync(r => r.MarketplaceItem.SellerId == userId);
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get reports count for user")
                    .CausedBy(ex));
            }
        }
    }
}
