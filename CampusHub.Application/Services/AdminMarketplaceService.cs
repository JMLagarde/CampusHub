using CampusHub.Application.DTO.Admin;
using CampusHub.Application.DTO.Marketplace;
using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace CampusHub.Application.Services
{
    public class AdminMarketplaceService : IAdminMarketplaceService
    {
        private readonly IAdminMarketplaceRepository _adminMarketplaceRepository;
        private readonly ILogger<AdminMarketplaceService> _logger;

        public AdminMarketplaceService(
            IAdminMarketplaceRepository adminMarketplaceRepository,
            ILogger<AdminMarketplaceService> _logger)
        {
            _adminMarketplaceRepository = adminMarketplaceRepository;
            this._logger = _logger;
        }

        public async Task<Result<List<MarketplaceItemDto>>> GetAllItemsAsync()
        {
            try
            {
                var itemsResult = await _adminMarketplaceRepository.GetAllItemsAsync();
                if (itemsResult.IsFailed)
                {
                    return Result.Fail(itemsResult.Errors);
                }

                var itemDtos = itemsResult.Value.Select(MapToDto).ToList();
                return Result.Ok(itemDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all marketplace items for admin");
                return Result.Fail(new Error("An error occurred while retrieving marketplace items")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<List<ReportDto>>> GetAllReportsAsync()
        {
            try
            {
                var reportsResult = await _adminMarketplaceRepository.GetAllReportsAsync();
                if (reportsResult.IsFailed)
                {
                    return Result.Fail(reportsResult.Errors);
                }

                var reportDtos = reportsResult.Value.Select(MapToReportDto).ToList();
                return Result.Ok(reportDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all reports for admin");
                return Result.Fail(new Error("An error occurred while retrieving reports")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<AdminMarketplaceStatsDto>> GetMarketplaceStatsAsync()
        {
            try
            {
                var totalResult = await _adminMarketplaceRepository.GetTotalListingsCountAsync();
                var activeResult = await _adminMarketplaceRepository.GetActiveListingsCountAsync();
                var soldResult = await _adminMarketplaceRepository.GetSoldListingsCountAsync();
                var flaggedResult = await _adminMarketplaceRepository.GetFlaggedListingsCountAsync();

                if (totalResult.IsFailed || activeResult.IsFailed || soldResult.IsFailed || flaggedResult.IsFailed)
                {
                    return Result.Fail(new Error("Failed to retrieve marketplace statistics"));
                }

                var stats = new AdminMarketplaceStatsDto
                {
                    TotalListings = totalResult.Value,
                    ActiveListings = activeResult.Value,
                    SoldListings = soldResult.Value,
                    FlaggedListings = flaggedResult.Value
                };

                return Result.Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving marketplace statistics");
                return Result.Fail(new Error("An error occurred while retrieving marketplace statistics")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> UpdateItemStatusAsync(int itemId, MarketplaceItemStatus status)
        {
            try
            {
                if (itemId <= 0)
                {
                    return Result.Fail(new Error("Invalid item ID"));
                }

                return await _adminMarketplaceRepository.UpdateItemStatusAsync(itemId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item status for {ItemId}", itemId);
                return Result.Fail(new Error("An error occurred while updating item status")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> UpdateReportStatusAsync(int reportId, ReportStatus status, int adminUserId, string? adminNotes = null)
        {
            try
            {
                if (reportId <= 0 || adminUserId <= 0)
                {
                    return Result.Fail(new Error("Invalid report ID or admin user ID"));
                }

                return await _adminMarketplaceRepository.UpdateReportStatusAsync(reportId, status, adminUserId, adminNotes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating report status for {ReportId}", reportId);
                return Result.Fail(new Error("An error occurred while updating report status")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> DeleteItemAsync(int itemId)
        {
            try
            {
                if (itemId <= 0)
                {
                    return Result.Fail(new Error("Invalid item ID"));
                }

                return await _adminMarketplaceRepository.DeleteItemAsync(itemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item {ItemId}", itemId);
                return Result.Fail(new Error("An error occurred while deleting item")
                    .CausedBy(ex));
            }
        }

        private static MarketplaceItemDto MapToDto(MarketplaceItem item)
        {
            return new MarketplaceItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Price = item.Price,
                Category = item.Category,
                Condition = item.Condition,
                MeetupPreference = item.MeetupPreference,
                Location = GetLocationDisplayName(item.Location),
                ImageUrl = item.ImageUrl,
                SellerName = item.SellerName,
                SellerId = item.SellerId,
                ContactNumber = item.ContactNumber ?? string.Empty,
                LikesCount = item.LikesCount,
                CreatedDate = item.CreatedDate,
                Status = item.Status,
                UpdatedAt = item.UpdatedDate,
                CreatedAt = item.CreatedDate,
                TimeAgo = GetTimeAgo(item.CreatedDate)
            };
        }

        private static ReportDto MapToReportDto(Report report)
        {
            return new ReportDto
            {
                Id = report.Id,
                MarketplaceItemId = report.MarketplaceItemId,
                ItemTitle = report.MarketplaceItem?.Title ?? "Unknown Item",
                ReporterId = report.ReporterUserID,
                ReporterName = report.Reporter?.GetDisplayName() ?? "Unknown User",
                Reason = report.Reason,
                Description = report.Description,
                Status = report.Status.ToString(),
                CreatedAt = report.CreatedAt,
                AdminNotes = report.AdminNotes,
                AdminUserId = report.ResolvedByUserId
            };
        }

        private static string GetLocationDisplayName(CampusLocation location)
        {
            return location switch
            {
                CampusLocation.MainCampus => "Main Campus",
                CampusLocation.Congressional => "Congressional Extension Campus",
                CampusLocation.BagongSilang => "Bagong Silang Extension Campus",
                CampusLocation.Camarin => "Camarin Extension Campus",
                _ => "Unknown"
            };
        }

        private static string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.UtcNow - date;

            return timeSpan.TotalDays switch
            {
                < 1 => timeSpan.TotalHours < 1 ? "Just now" : $"{(int)timeSpan.TotalHours} hours ago",
                < 7 => $"{(int)timeSpan.TotalDays} days ago",
                < 30 => $"{(int)(timeSpan.TotalDays / 7)} weeks ago",
                _ => $"{(int)(timeSpan.TotalDays / 30)} months ago"
            };
        }
    }
}
