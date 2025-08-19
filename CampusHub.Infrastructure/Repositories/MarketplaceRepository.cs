using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using CampusHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Infrastructure.Repositories
{
    public class MarketplaceRepository : IMarketplaceRepository
    {
        private readonly ApplicationDbContext _context;

        public MarketplaceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MarketplaceItem>> GetAllAsync()
        {
            return await _context.MarketplaceItems
                .AsNoTracking()
                .Where(x => x.IsActive)
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
            var item = await GetByIdAsync(id);
            if (item == null) return false;

            item.IsActive = false;
            item.UpdatedDate = DateTime.UtcNow;
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
                .Where(x => x.IsActive && x.Location == location)
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
    }
}
