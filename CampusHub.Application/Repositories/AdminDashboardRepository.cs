using CampusHub.Application.DTO.Admin;
using CampusHub.Application.Interfaces;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.Application.Repositories
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly IApplicationDbContext _context;

        public AdminDashboardRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<int>> GetTotalStudentsCountAsync()
        {
            try
            {
                var count = await _context.Users.CountAsync();
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get total students count")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<int>> GetPublishedEventsCountAsync()
        {
            try
            {
                var count = await _context.Events.CountAsync();
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get published events count")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<List<CategoryDistributionDto>>> GetCategoryDistributionAsync()
        {
            try
            {
                var categoryCounts = await _context.MarketplaceItems
                    .Where(x => x.Category != null)
                    .GroupBy(x => x.Category)
                    .Select(g => new
                    {
                        Category = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();

                var total = categoryCounts.Sum(x => x.Count);
                var distribution = categoryCounts.Select(x => new CategoryDistributionDto
                {
                    Category = x.Category.ToString(),
                    Count = x.Count,
                    Percentage = total > 0 ? Math.Round((double)x.Count / total * 100, 1) : 0
                }).ToList();

                return Result.Ok(distribution);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get category distribution")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<List<CollegeDistributionDto>>> GetCollegeDistributionAsync()
        {
            try
            {
                var collegeCounts = await _context.Users
                    .Include(u => u.Program)
                    .ThenInclude(p => p.College)
                    .Where(u => u.Program != null && u.Program.College != null)
                    .GroupBy(u => u.Program.College.CollegeName)
                    .Select(g => new
                    {
                        College = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync();

                var distribution = collegeCounts.Select(x => new CollegeDistributionDto
                {
                    College = x.College,
                    Count = x.Count
                }).ToList();

                return Result.Ok(distribution);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get college distribution")
                    .CausedBy(ex));
            }
        }
    }
}
