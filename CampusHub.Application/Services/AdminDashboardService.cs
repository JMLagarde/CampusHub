using CampusHub.Application.DTO.Admin;
using CampusHub.Application.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace CampusHub.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository _dashboardRepository;
        private readonly IAdminMarketplaceService _marketplaceService;
        private readonly IAdminReportsService _reportsService;
        private readonly ILogger<AdminDashboardService> _logger;

        public AdminDashboardService(
            IAdminDashboardRepository dashboardRepository,
            IAdminMarketplaceService marketplaceService,
            IAdminReportsService reportsService,
            ILogger<AdminDashboardService> logger)
        {
            _dashboardRepository = dashboardRepository;
            _marketplaceService = marketplaceService;
            _reportsService = reportsService;
            _logger = logger;
        }

        public async Task<Result<AdminDashboardStatsDto>> GetDashboardStatsAsync()
        {
            try
            {
                // ? FIXED: Execute queries SEQUENTIALLY to avoid DbContext concurrency
                var totalStudents = await _dashboardRepository.GetTotalStudentsCountAsync();
                if (totalStudents.IsFailed)
                    return Result.Fail(totalStudents.Errors);

                var publishedEvents = await _dashboardRepository.GetPublishedEventsCountAsync();
                if (publishedEvents.IsFailed)
                    return Result.Fail(publishedEvents.Errors);

                var marketplaceStats = await _marketplaceService.GetMarketplaceStatsAsync();
                if (marketplaceStats.IsFailed)
                    return Result.Fail(marketplaceStats.Errors);

                var reportsStats = await _reportsService.GetReportsStatsAsync();
                if (reportsStats.IsFailed)
                    return Result.Fail(reportsStats.Errors);

                var categoryDistribution = await _dashboardRepository.GetCategoryDistributionAsync();
                if (categoryDistribution.IsFailed)
                    return Result.Fail(categoryDistribution.Errors);

                var collegeDistribution = await _dashboardRepository.GetCollegeDistributionAsync();
                if (collegeDistribution.IsFailed)
                    return Result.Fail(collegeDistribution.Errors);

                var stats = new AdminDashboardStatsDto
                {
                    TotalStudents = totalStudents.Value,
                    PublishedEvents = publishedEvents.Value,
                    ActiveListings = marketplaceStats.Value.ActiveListings,
                    PendingReports = reportsStats.Value.PendingReports,
                    CategoryDistribution = categoryDistribution.Value,
                    CollegeDistribution = collegeDistribution.Value
                };

                return Result.Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard statistics");
                return Result.Fail(new Error("An error occurred while retrieving dashboard statistics")
                    .CausedBy(ex));
            }
        }
    }
}