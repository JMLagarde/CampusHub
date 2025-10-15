using CampusHub.Domain.Entities;
using FluentResults;

namespace CampusHub.Application.Interfaces
{
    public interface IAdminMarketplaceRepository
    {
        Task<Result<List<MarketplaceItem>>> GetAllItemsAsync();
        Task<Result<List<Report>>> GetAllReportsAsync();
        Task<Result<MarketplaceItem>> GetItemByIdAsync(int id);
        Task<Result> UpdateItemStatusAsync(int itemId, MarketplaceItemStatus status);
        Task<Result> UpdateReportStatusAsync(int reportId, ReportStatus status, int adminUserId, string? adminNotes = null);
        Task<Result<int>> GetTotalListingsCountAsync();
        Task<Result<int>> GetActiveListingsCountAsync();
        Task<Result<int>> GetSoldListingsCountAsync();
        Task<Result<int>> GetFlaggedListingsCountAsync();
    }
}
