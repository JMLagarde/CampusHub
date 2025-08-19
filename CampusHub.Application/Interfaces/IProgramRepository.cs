using CampusHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Application.Interfaces
{
    public interface IProgramRepository
    {
        Task<List<ProgramEntity>> GetAllAsync();
        Task<ProgramEntity?> GetByIdAsync(int id);
        Task<int> AddAsync(ProgramEntity program);
        Task UpdateAsync(ProgramEntity program);
        Task DeleteAsync(int id);
    }
}
