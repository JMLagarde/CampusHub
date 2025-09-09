using CampusHub.Domain.Entities;

namespace CampusHub.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<List<User>> GetAllAsync();
        Task<int> AddAsync(User user);
        Task<bool> UsernameExistsAsync(string username);
        Task<User?> GetUserWithDetailsAsync(int id); 
        Task<List<User>> GetAllUsersWithDetailsAsync();
        Task<bool> UpdateAsync(User user);
    }
}
