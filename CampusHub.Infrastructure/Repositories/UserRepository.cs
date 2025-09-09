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
        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);

                var result = await _context.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging configured
                // _logger?.LogError(ex, "Error updating user with ID {UserId}", user.UserID);
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }
    }
}
