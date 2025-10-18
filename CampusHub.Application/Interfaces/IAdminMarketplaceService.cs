using CampusHub.Application.DTO;
using CampusHub.Domain.Entities;
using FluentResults;

namespace CampusHub.Application.Interfaces
{
    public interface IAdminMarketplaceService
    {
        Task<Result<List<MarketplaceItemDto>>> GetAllItemsAsync();
        Task<Result<List<ReportDto>>> GetAllReportsAsync();
        Task<Result<AdminMarketplaceStatsDto>> GetMarketplaceStatsAsync();
        Task<Result> UpdateItemStatusAsync(int itemId, MarketplaceItemStatus status);
        Task<Result> UpdateReportStatusAsync(int reportId, ReportStatus status, int adminUserId, string? adminNotes = null);
        Task<Result> DeleteItemAsync(int itemId);
    }
}
