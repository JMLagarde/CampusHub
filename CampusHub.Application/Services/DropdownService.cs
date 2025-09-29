using CampusHub.Application.DTO;
using CampusHub.Application.Interfaces;

namespace CampusHub.Application.Services
{
    public class DropdownService : IDropdownService
    {
        private readonly IYearLevelRepository _yearLevelRepository;
        private readonly IProgramRepository _programRepository;

        public DropdownService(IYearLevelRepository yearLevelRepository, IProgramRepository programRepository)
        {
            _yearLevelRepository = yearLevelRepository;
            _programRepository = programRepository;
        }

        public async Task<List<YearLevelDto>> GetYearLevelsAsync()
        {
            var yearLevels = await _yearLevelRepository.GetAllAsync();

            return yearLevels.Select(y => new YearLevelDto
            {
                Id = y.Id,
                Name = y.Name
            }).ToList();
        }

        public async Task<List<ProgramDto>> GetProgramsAsync()
        {
            var programs = await _programRepository.GetAllAsync();

            return programs.Select(p => new ProgramDto
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
        }
    }
}