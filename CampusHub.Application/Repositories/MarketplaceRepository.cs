using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.Application.Repositories
{
    public class MarketplaceRepository : IMarketplaceRepository
    {
        private readonly IApplicationDbContext _context;

        public MarketplaceRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MarketplaceItem>> GetAllAsync()
        {
            return await _context.MarketplaceItems
                .AsNoTracking()
                .Where(x => x.IsActive && x.Status != MarketplaceItemStatus.Sold)
                .Include(x => x.Likes)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<MarketplaceItem?> GetByIdAsync(int id)
        {
            return await _context.MarketplaceItems
                .Include(x => x.Likes)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<MarketplaceItem> CreateAsync(MarketplaceItem item)
        {
            _context.MarketplaceItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<MarketplaceItem> UpdateAsync(MarketplaceItem item)
        {
            _context.MarketplaceItems.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.MarketplaceItems.FindAsync(id);
            if (item == null) return false;

            _context.MarketplaceItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleLikeAsync(int itemId, int userId)
        {
            var existingLike = await _context.MarketplaceLikes
                .FirstOrDefaultAsync(x => x.MarketplaceItemId == itemId && x.UserId == userId);

            if (existingLike != null)
            {
                _context.MarketplaceLikes.Remove(existingLike);

                var item = await _context.MarketplaceItems.FindAsync(itemId);
                if (item != null)
                {
                    item.LikesCount = Math.Max(0, item.LikesCount - 1);
                }
            }
            else
            {
                var newLike = new MarketplaceLike
                {
                    MarketplaceItemId = itemId,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };
                _context.MarketplaceLikes.Add(newLike);

                var item = await _context.MarketplaceItems.FindAsync(itemId);
                if (item != null)
                {
                    item.LikesCount++;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsLikedByUserAsync(int itemId, int userId)
        {
            return await _context.MarketplaceLikes
                .AnyAsync(x => x.MarketplaceItemId == itemId && x.UserId == userId);
        }

        public async Task<int> GetLikesCountAsync(int itemId)
        {
            return await _context.MarketplaceLikes
                .CountAsync(x => x.MarketplaceItemId == itemId);
        }

        public async Task<IEnumerable<MarketplaceItem>> GetByLocationAsync(CampusLocation location)
        {
            return await _context.MarketplaceItems
                .Where(x => x.IsActive && x.Status != MarketplaceItemStatus.Sold && x.Location == location)
                .Include(x => x.Likes)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MarketplaceItem>> GetBySellerAsync(int sellerId)
        {
            return await _context.MarketplaceItems
                .Where(x => x.IsActive && x.SellerId == sellerId)
                .Include(x => x.Likes)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<MarketplaceItem>> GetUserWishlistAsync(int userId)
        {
            return await _context.MarketplaceLikes
                .Where(like => like.UserId == userId)
                .Include(like => like.MarketplaceItem)
                .ThenInclude(item => item.Likes)
                .Where(like => like.MarketplaceItem.IsActive)
                .Select(like => like.MarketplaceItem)
                .OrderByDescending(item => item.CreatedDate)
                .ToListAsync();
        }

        public async Task<int> GetUserWishlistCountAsync(int userId)
        {
            return await _context.MarketplaceLikes
                .Where(like => like.UserId == userId)
                .CountAsync(like => like.MarketplaceItem.IsActive);
        }

        public async Task<bool> RemoveFromWishlistAsync(int itemId, int userId)
        {
            var like = await _context.MarketplaceLikes
                .FirstOrDefaultAsync(x => x.MarketplaceItemId == itemId && x.UserId == userId);

            if (like != null)
            {
                _context.MarketplaceLikes.Remove(like);

                var item = await _context.MarketplaceItems.FindAsync(itemId);
                if (item != null)
                {
                    item.LikesCount = Math.Max(0, item.LikesCount - 1);
                }

                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ReportItemAsync(int itemId, int reporterId, string reason, string? description)
        {
            try
            {
                var report = new Report
                {
                    MarketplaceItemId = itemId,
                    ReporterUserID = reporterId,
                    Reason = reason,
                    Description = description,
                    CreatedAt = DateTime.UtcNow,
                    Status = ReportStatus.Pending
                };

                await _context.Reports.AddAsync(report);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Report>> GetReportsAsync()
        {
            return await _context.Reports
                .Include(r => r.MarketplaceItem)
                .Include(r => r.Reporter)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetReportsByItemAsync(int itemId)
        {
            return await _context.Reports
                .Include(r => r.Reporter)
                .Where(r => r.MarketplaceItemId == itemId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Report?> GetReportByIdAsync(int reportId)
        {
            return await _context.Reports
                .Include(r => r.MarketplaceItem)
                .Include(r => r.Reporter)
                .FirstOrDefaultAsync(r => r.Id == reportId);
        }

        public async Task<bool> UpdateReportStatusAsync(int reportId, ReportStatus status, int adminUserId, string? adminNotes = null)
        {
            try
            {
                var report = await _context.Reports.FindAsync(reportId);
                if (report == null) return false;

                report.Status = status;
                report.ResolvedAt = DateTime.UtcNow;
                report.ResolvedByUserId = adminUserId;
                report.AdminNotes = adminNotes;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}