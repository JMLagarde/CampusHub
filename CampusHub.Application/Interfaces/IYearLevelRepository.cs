using CampusHub.Domain.Entities;

namespace CampusHub.Application.Interfaces
{
    public interface IYearLevelRepository
    {
        Task<List<YearLevel>> GetAllAsync();
        Task<YearLevel?> GetByIdAsync(int id);
        Task<int> AddAsync(YearLevel yearLevel);
        Task UpdateAsync(YearLevel yearLevel);
        Task DeleteAsync(int id);
    }
}
