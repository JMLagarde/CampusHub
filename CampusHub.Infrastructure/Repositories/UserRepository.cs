using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using CampusHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        // task result 
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<int> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.UserID;
        }
        
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserWithDetailsAsync(int id)
        {
            return await _context.Users
                .Include(u => u.YearLevel)
                .Include(u => u.Program)
                .FirstOrDefaultAsync(u => u.UserID == id);
        }

        public async Task<List<User>> GetAllUsersWithDetailsAsync()
        {
            return await _context.Users
                .Include(u => u.YearLevel)
                .Include(u => u.Program)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }
    }
}
