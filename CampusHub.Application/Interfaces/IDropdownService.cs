using CampusHub.Application.DTO;

namespace CampusHub.Application.Interfaces
{
    public interface IDropdownService
    {
        Task<List<YearLevelDto>> GetYearLevelsAsync();
        Task<List<ProgramDto>> GetProgramsAsync();
    }
}
