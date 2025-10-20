using CampusHub.Application.DTO.Admin;
using CampusHub.Application.DTO.Marketplace;
using CampusHub.Domain.Entities;
using FluentResults;

namespace CampusHub.Application.Interfaces
{
    public interface IAdminReportsService
    {
        Task<Result<List<ReportDto>>> GetAllReportsAsync();
        Task<Result<AdminReportsStatsDto>> GetReportsStatsAsync();
        Task<Result> UpdateReportStatusAsync(int reportId, ReportStatus status, int adminUserId, string? adminNotes = null);
        Task<Result> FlagItemAsync(int itemId, int adminUserId);
    }
}
