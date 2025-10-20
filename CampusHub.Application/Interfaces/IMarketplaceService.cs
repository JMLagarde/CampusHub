using FluentResults;
using CampusHub.Domain.Entities;
using CampusHub.Application.DTO.Marketplace;
using CampusHub.Application.DTO.User;
using CampusHub.Application.DTO.Common;

namespace CampusHub.Application.Interfaces
{
    public interface IMarketplaceService
    {
        Task<Result<IEnumerable<MarketplaceItemDto>>> GetAllItemsAsync(int? currentUserId = null);
        Task<Result<MarketplaceItemDto>> GetItemByIdAsync(int id, int? currentUserId = null);
        Task<Result<IEnumerable<MarketplaceItemDto>>> GetItemsByLocationAsync(CampusLocation location, int? currentUserId = null);
        Task<Result<IEnumerable<MarketplaceItemDto>>> GetItemsBySellerAsync(int sellerId, int? currentUserId = null);
        Task<Result<List<MarketplaceItemDto>>> GetUserListingsAsync(int userId);
        Task<Result<IEnumerable<ReportDto>>> GetAllReportsAsync();
        Task<Result<IEnumerable<ReportDto>>> GetReportsByItemAsync(int itemId);
        Task<Result<List<MarketplaceItemDto>>> GetUserItemsAsync(int userId);
        Task<Result<List<MarketplaceItemDto>>> GetUserWishlistAsync(int userId);
        Task<Result<int>> GetUserWishlistCountAsync(int userId);
        Task<Result<UserStatsDto>> GetUserStatsAsync(int userId);
        Task<Result<MarketplaceItemDto>> CreateItemAsync(CreateMarketplaceItemDto dto);
        Task<Result<MarketplaceItemDto>> UpdateItemAsync(UpdateMarketplaceItemDto dto);
        Task<Result<bool>> DeleteItemAsync(int id, int userId);
        Task<Result<bool>> ToggleLikeAsync(int itemId, int userId);
        Task<Result> ToggleLikeAsync(ToggleLikeDto toggleDto);
        Task<Result> UpdateItemStatusAsync(int itemId, MarketplaceItemStatus status);
        Task<Result<bool>> RemoveFromWishlistAsync(int itemId, int userId);
        Task<Result<bool>> ReportItemAsync(CreateReportDto reportDto);
        Task<Result<bool>> UpdateReportStatusAsync(int reportId, ReportStatus status, int adminUserId, string? adminNotes = null);
        Task<Result> MarkItemAvailableAsync(ItemStatusOperationDto dto);
        Task<Result> MarkItemSoldAsync(ItemStatusOperationDto dto);
    }
}