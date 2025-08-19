using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using CampusHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.Infrastructure.Repositories
{
    public class YearLevelRepository : IYearLevelRepository
    {
        private readonly ApplicationDbContext _context;

        public YearLevelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<YearLevel>> GetAllAsync()
        {
            return await _context.YearLevels
                .OrderBy(y => y.Name)
                .ToListAsync();
        }

        public async Task<YearLevel?> GetByIdAsync(int id)
        {
            return await _context.YearLevels
                .FirstOrDefaultAsync(y => y.Id == id);
        }

        public async Task<int> AddAsync(YearLevel yearLevel)
        {
            _context.YearLevels.Add(yearLevel);
            await _context.SaveChangesAsync();
            return yearLevel.Id;
        }

        public async Task UpdateAsync(YearLevel yearLevel)
        {
            _context.YearLevels.Update(yearLevel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var yearLevel = await _context.YearLevels.FindAsync(id);
            if (yearLevel != null)
            {
                _context.YearLevels.Remove(yearLevel);
                await _context.SaveChangesAsync();
            }
        }
    }
}
