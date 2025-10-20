using CampusHub.Application.DTO.Common;
using CampusHub.Application.DTO.User;
using FluentResults;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampusHub.Application.Interfaces
{
    public interface IUserService
    {
        // Operations that can fail - return Result<T>
        Task<Result<int>> CreateUserAsync(CreateUserDto dto);
        Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto);
        Task<Result<bool>> UpdateUserProfileAsync(UpdateUserProfileDto userDto);
        Task<Result<bool>> ValidateUserAsync(string username, string password);

        // Simple operation - return Result
        Task<Result> LogoutAsync();

        // Read operations - unchanged (no FluentResults)
        Task<CurrentUserDto?> GetUserByIdAsync(int userId);
        Task<CreateUserDto?> GetUserByUsernameAsync(string username);
        Task<List<CreateUserDto>> GetAllUsersAsync();
        Task<bool> UsernameExistsAsync(string username);
        Task<CurrentUserDto?> GetCurrentUserAsync(int userId);
    }
}