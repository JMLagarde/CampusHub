using CampusHub.Domain.Entities;
using FluentResults;

namespace CampusHub.Application.Interfaces
{
    public interface IAdminReportsRepository
    {
        Task<Result<List<Report>>> GetAllReportsAsync();
        Task<Result<Report>> GetReportByIdAsync(int id);
        Task<Result> UpdateReportStatusAsync(int reportId, ReportStatus status, int adminUserId, string? adminNotes = null);
        Task<Result<int>> GetTotalReportsCountAsync();
        Task<Result<int>> GetPendingReportsCountAsync();
        Task<Result<int>> GetResolvedReportsCountAsync();
    }
}
