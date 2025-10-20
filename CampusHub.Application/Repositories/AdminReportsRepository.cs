using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.Application.Repositories
{
    public class AdminReportsRepository : IAdminReportsRepository
    {
        private readonly IApplicationDbContext _context;

        public AdminReportsRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Report>>> GetAllReportsAsync()
        {
            try
            {
                var reports = await _context.Reports
                    .AsNoTracking()
                    .Include(r => r.MarketplaceItem)
                    .Include(r => r.Reporter)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return Result.Ok(reports);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to retrieve reports")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<Report>> GetReportByIdAsync(int id)
        {
            try
            {
                var report = await _context.Reports
                    .Include(r => r.MarketplaceItem)
                    .Include(r => r.Reporter)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (report == null)
                {
                    return Result.Fail(new Error("Report not found"));
                }

                return Result.Ok(report);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to retrieve report")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> UpdateReportStatusAsync(int reportId, ReportStatus status, int adminUserId, string? adminNotes = null)
        {
            try
            {
                var report = await _context.Reports.FindAsync(reportId);
                if (report == null)
                {
                    return Result.Fail(new Error("Report not found"));
                }

                report.Status = status;
                report.ResolvedAt = DateTime.UtcNow;
                report.ResolvedByUserId = adminUserId;
                report.AdminNotes = adminNotes;

                await _context.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to update report status")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<int>> GetTotalReportsCountAsync()
        {
            try
            {
                var count = await _context.Reports.CountAsync();
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get total reports count")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<int>> GetPendingReportsCountAsync()
        {
            try
            {
                var count = await _context.Reports
                    .CountAsync(r => r.Status == ReportStatus.Pending);
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get pending reports count")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<int>> GetResolvedReportsCountAsync()
        {
            try
            {
                var count = await _context.Reports
                    .CountAsync(r => r.Status == ReportStatus.Resolved);
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get resolved reports count")
                    .CausedBy(ex));
            }
        }
    }
}
