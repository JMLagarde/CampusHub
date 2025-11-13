using CampusHub.Application.DTO.Admin;
using FluentResults;

namespace CampusHub.Application.Interfaces
{
    public interface IAdminDashboardRepository
    {
        Task<Result<int>> GetTotalStudentsCountAsync();
        Task<Result<int>> GetPublishedEventsCountAsync();
        Task<Result<List<CategoryDistributionDto>>> GetCategoryDistributionAsync();
        Task<Result<List<CollegeDistributionDto>>> GetCollegeDistributionAsync();
    }
}
