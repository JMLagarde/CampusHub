using CampusHub.Application.DTO.Common;

namespace CampusHub.Application.Interfaces
{
    public interface IDropdownService
    {
        Task<List<YearLevelDto>> GetYearLevelsAsync();
        Task<List<ProgramDto>> GetProgramsAsync();
    }
}
