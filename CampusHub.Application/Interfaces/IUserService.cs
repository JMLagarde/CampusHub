using CampusHub.Application.DTO;

namespace CampusHub.Application.Interfaces
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(CreateUserDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> ValidateUserAsync(string username, string password);
        Task LogoutAsync();
        Task<CurrentUserDto?> GetUserByIdAsync(int userId);
        Task<CreateUserDto?> GetUserByUsernameAsync(string username);
        Task<List<CreateUserDto>> GetAllUsersAsync();
        Task<bool> UsernameExistsAsync(string username);
        Task<CurrentUserDto?> GetCurrentUserAsync(int userId);
    }
}