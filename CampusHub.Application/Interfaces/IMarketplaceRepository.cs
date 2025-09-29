using CampusHub.Application.DTO;
using CampusHub.Domain.Entities;

namespace CampusHub.Application.Interfaces
{
    public interface IMarketplaceRepository
    {
        Task<IEnumerable<MarketplaceItem>> GetAllAsync();
        Task<MarketplaceItem?> GetByIdAsync(int id);
        Task<MarketplaceItem> CreateAsync(MarketplaceItem item);
        Task<MarketplaceItem> UpdateAsync(MarketplaceItem item);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsLikedByUserAsync(int itemId, int userId);
        Task<bool> ToggleLikeAsync(int itemId, int userId);
        Task<IEnumerable<MarketplaceItem>> GetByLocationAsync(CampusLocation location);
        Task<IEnumerable<MarketplaceItem>> GetBySellerAsync(int sellerId);
        Task<IEnumerable<MarketplaceItem>> GetUserWishlistAsync(int userId);
        Task<int> GetUserWishlistCountAsync(int userId);
        Task<bool> RemoveFromWishlistAsync(int itemId, int userId);
        Task<bool> ReportItemAsync(int itemId, int reporterId, string reason, string? description);
        Task<IEnumerable<Report>> GetReportsAsync();
        Task<IEnumerable<Report>> GetReportsByItemAsync(int itemId);
        Task<Report?> GetReportByIdAsync(int reportId);
        Task<bool> UpdateReportStatusAsync(int reportId, ReportStatus status, int adminUserId, string? adminNotes = null);
        Task SaveChangesAsync();
    }
}