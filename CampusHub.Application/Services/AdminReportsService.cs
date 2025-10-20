using CampusHub.Application.DTO.Admin;
using CampusHub.Application.DTO.Marketplace;
using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace CampusHub.Application.Services
{
    public class AdminReportsService : IAdminReportsService
    {
        private readonly IAdminReportsRepository _adminReportsRepository;
        private readonly IAdminMarketplaceRepository _adminMarketplaceRepository;
        private readonly ILogger<AdminReportsService> _logger;

        public AdminReportsService(
            IAdminReportsRepository adminReportsRepository,
            IAdminMarketplaceRepository adminMarketplaceRepository,
            ILogger<AdminReportsService> _logger)
        {
            _adminReportsRepository = adminReportsRepository;
            _adminMarketplaceRepository = adminMarketplaceRepository;
            this._logger = _logger;
        }

        public async Task<Result<List<ReportDto>>> GetAllReportsAsync()
        {
            try
            {
                var reportsResult = await _adminReportsRepository.GetAllReportsAsync();
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

        public async Task<Result<AdminReportsStatsDto>> GetReportsStatsAsync()
        {
            try
            {
                var totalResult = await _adminReportsRepository.GetTotalReportsCountAsync();
                var pendingResult = await _adminReportsRepository.GetPendingReportsCountAsync();
                var resolvedResult = await _adminReportsRepository.GetResolvedReportsCountAsync();

                if (totalResult.IsFailed || pendingResult.IsFailed || resolvedResult.IsFailed)
                {
                    return Result.Fail(new Error("Failed to retrieve reports statistics"));
                }

                var stats = new AdminReportsStatsDto
                {
                    TotalReports = totalResult.Value,
                    PendingReports = pendingResult.Value,
                    ResolvedReports = resolvedResult.Value
                };

                return Result.Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reports statistics");
                return Result.Fail(new Error("An error occurred while retrieving reports statistics")
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

                return await _adminReportsRepository.UpdateReportStatusAsync(reportId, status, adminUserId, adminNotes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating report status for {ReportId}", reportId);
                return Result.Fail(new Error("An error occurred while updating report status")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> FlagItemAsync(int itemId, int adminUserId)
        {
            try
            {
                if (itemId <= 0 || adminUserId <= 0)
                {
                    return Result.Fail(new Error("Invalid item ID or admin user ID"));
                }

                return await _adminMarketplaceRepository.UpdateItemStatusAsync(itemId, MarketplaceItemStatus.Flagged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error flagging item {ItemId}", itemId);
                return Result.Fail(new Error("An error occurred while flagging item")
                    .CausedBy(ex));
            }
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
    }
}
