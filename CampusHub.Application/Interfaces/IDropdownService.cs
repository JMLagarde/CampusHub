using CampusHub.Application.DTO;
using CampusHub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Application.Interfaces
{
    public interface IDropdownService
    {
        Task<List<YearLevelDto>> GetYearLevelsAsync();
        Task<List<ProgramDto>> GetProgramsAsync();
    }
}
