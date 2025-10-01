using CampusHub.Application.DTO;
using CampusHub.Domain.Entities;


namespace CampusHub.Application.Interfaces
{
    public interface IMarketplaceService
    {
        Task<IEnumerable<MarketplaceItemDto>> GetAllItemsAsync(int? currentUserId = null);
        Task<MarketplaceItemDto?> GetItemByIdAsync(int id, int? currentUserId = null);
        Task<MarketplaceItemDto> CreateItemAsync(CreateMarketplaceItemDto dto); //task result
        Task<MarketplaceItemDto> UpdateItemAsync(UpdateMarketplaceItemDto dto); // task result
        Task<bool> DeleteItemAsync(int id, int userId);
        Task<bool> ToggleLikeAsync(int itemId, int userId);
        Task<IEnumerable<MarketplaceItemDto>> GetItemsByLocationAsync(CampusLocation location, int? currentUserId = null);
        Task<IEnumerable<MarketplaceItemDto>> GetItemsBySellerAsync(int sellerId, int? currentUserId = null);
        Task<List<MarketplaceItemDto>> GetUserItemsAsync(int userId);
        Task ToggleLikeAsync(ToggleLikeDto toggleDto);
        Task<List<MarketplaceItemDto>> GetUserListingsAsync(int userId);
        Task UpdateItemStatusAsync(int itemId, MarketplaceItemStatus status);
        Task<List<MarketplaceItemDto>> GetUserWishlistAsync(int userId);
        Task<int> GetUserWishlistCountAsync(int userId);
        Task<bool> RemoveFromWishlistAsync(int itemId, int userId);
        Task<bool> ReportItemAsync(CreateReportDto reportDto);
        Task<IEnumerable<ReportDto>> GetAllReportsAsync();
        Task<IEnumerable<ReportDto>> GetReportsByItemAsync(int itemId);
        Task<bool> UpdateReportStatusAsync(int reportId, ReportStatus status, int adminUserId, string? adminNotes = null);
        Task MarkItemAvailableAsync(ItemStatusOperationDto dto);
        Task MarkItemSoldAsync(ItemStatusOperationDto dto);
        Task<UserStatsDto> GetUserStatsAsync(int userId);

    }
}
