using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using CampusHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.Infrastructure.Repositories
{
    public class ProgramRepository : IProgramRepository
    {
        private readonly ApplicationDbContext _context;

        public ProgramRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProgramEntity>> GetAllAsync()
        {
            return await _context.Programs
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<ProgramEntity?> GetByIdAsync(int id)
        {
            return await _context.Programs
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> AddAsync(ProgramEntity program)
        {
            _context.Programs.Add(program);
            await _context.SaveChangesAsync();
            return program.Id;
        }

        public async Task UpdateAsync(ProgramEntity program)
        {
            _context.Programs.Update(program);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var program = await _context.Programs.FindAsync(id);
            if (program != null)
            {
                _context.Programs.Remove(program);
                await _context.SaveChangesAsync();
            }
        }
    }
}
