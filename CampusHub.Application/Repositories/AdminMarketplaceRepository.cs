using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.Application.Repositories
{
    public class AdminMarketplaceRepository : IAdminMarketplaceRepository
    {
        private readonly IApplicationDbContext _context;

        public AdminMarketplaceRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<MarketplaceItem>>> GetAllItemsAsync()
        {
            try
            {
                var items = await _context.MarketplaceItems
                    .AsNoTracking()
                    .Include(x => x.Likes)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();

                return Result.Ok(items);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to retrieve marketplace items")
                    .CausedBy(ex));
            }
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

        public async Task<Result<MarketplaceItem>> GetItemByIdAsync(int id)
        {
            try
            {
                var item = await _context.MarketplaceItems
                    .Include(x => x.Likes)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                {
                    return Result.Fail(new Error("Item not found"));
                }

                return Result.Ok(item);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to retrieve item")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> UpdateItemStatusAsync(int itemId, MarketplaceItemStatus status)
        {
            try
            {
                var item = await _context.MarketplaceItems.FindAsync(itemId);
                if (item == null)
                {
                    return Result.Fail(new Error("Item not found"));
                }

                item.Status = status;
                item.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to update item status")
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

        public async Task<Result<int>> GetTotalListingsCountAsync()
        {
            try
            {
                var count = await _context.MarketplaceItems.CountAsync();
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get total listings count")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<int>> GetActiveListingsCountAsync()
        {
            try
            {
                var count = await _context.MarketplaceItems
                    .CountAsync(x => x.Status == MarketplaceItemStatus.Active);
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get active listings count")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<int>> GetSoldListingsCountAsync()
        {
            try
            {
                var count = await _context.MarketplaceItems
                    .CountAsync(x => x.Status == MarketplaceItemStatus.Sold);
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get sold listings count")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<int>> GetFlaggedListingsCountAsync()
        {
            try
            {
                var count = await _context.MarketplaceItems
                    .CountAsync(x => x.Status == MarketplaceItemStatus.Flagged);
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to get flagged listings count")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> DeleteItemAsync(int itemId)
        {
            try
            {
                var item = await _context.MarketplaceItems.FindAsync(itemId);
                if (item == null)
                {
                    return Result.Fail(new Error("Item not found"));
                }

                _context.MarketplaceItems.Remove(item);
                await _context.SaveChangesAsync();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(new Error("Failed to delete item")
                    .CausedBy(ex));
            }
        }
    }
}
