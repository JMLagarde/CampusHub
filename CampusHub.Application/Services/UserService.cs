using CampusHub.Application.DTO.Common;
using CampusHub.Application.DTO.User;
using CampusHub.Application.Helpers;
using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace CampusHub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Result<int>> CreateUserAsync(CreateUserDto dto)
        {
            try
            {
                var validation = ValidationHelper.ValidateCreateUser(dto);
                if (!validation.IsValid)
                {
                    return Result.Fail(new Error(string.Join("; ", validation.Errors))
                        .WithMetadata("StatusCode", 400));
                }

                if (await _userRepository.UsernameExistsAsync(dto.Username))
                {
                    return Result.Fail(new Error("Username already exists")
                        .WithMetadata("StatusCode", 409));
                }

                var user = new User
                {
                    Username = dto.Username,
                    FullName = dto.FullName,
                    StudentNumber = dto.StudentNumber,
                    Email = dto.Email,
                    ContactNumber = dto.ContactNumber,
                    Password = dto.Password,
                    Role = "Student",
                    DateRegistered = DateTime.Now,
                    YearLevelId = dto.YearLevelId,
                    ProgramID = dto.ProgramID
                };

                var result = await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Username}", dto.Username);
                return Result.Fail(new Error("An error occurred while creating the user")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var validation = ValidationHelper.ValidateLogin(loginDto);
                if (!validation.IsValid)
                {
                    return Result.Fail(new Error(string.Join("; ", validation.Errors))
                        .WithMetadata("StatusCode", 400));
                }
                var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
                if (user == null || user.Password != loginDto.Password)
                {
                    _logger.LogWarning("Invalid login attempt for username: {Username}", loginDto.Username);
                    return Result.Fail(new Error("Invalid username or password")
                        .WithMetadata("StatusCode", 401));
                }

                if (user.Status == "Banned")
                {
                    _logger.LogWarning("Login attempt for banned user: {Username}", loginDto.Username);
                    return Result.Fail(new Error("Account banned")
                        .WithMetadata("StatusCode", 403));
                }
                string redirectUrl = user.Role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true
                    ? "/admin/dashboard"
                    : "/main-marketplace";
                _logger.LogInformation("Login successful for user {Username} with role {Role}, redirecting to {RedirectUrl}",
                    user.Username, user.Role, redirectUrl);
                var loginResponse = new LoginResponseDto
                {
                    UserId = user.UserID,
                    Username = user.Username,
                    FullName = user.FullName,
                    Role = user.Role ?? "Student",
                    Email = user.Email,
                    IsSuccess = true,
                    Message = "Login successful",
                    RedirectUrl = redirectUrl
                };
                return Result.Ok(loginResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {Username}", loginDto.Username);
                return Result.Fail(new Error("An error occurred during login")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<bool>> ValidateUserAsync(string username, string password)
        {
            try
            {
                var validation = ValidationHelper.ValidateLogin(new LoginDto
                {
                    Username = username,
                    Password = password
                });

                if (!validation.IsValid)
                {
                    return Result.Ok(false);
                }

                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null)
                {
                    return Result.Ok(false);
                }

                return Result.Ok(user.Password == password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user: {Username}", username);
                return Result.Fail(new Error("An error occurred during validation")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public Task<Result> LogoutAsync()
        {
            return Task.FromResult(Result.Ok());
        }

        public async Task<Result<bool>> UpdateUserProfileAsync(UpdateUserProfileDto userDto)
        {
            try
            {
                var validation = ValidationHelper.ValidateUpdateUserProfile(userDto);
                if (!validation.IsValid)
                {
                    return Result.Fail(new Error(string.Join("; ", validation.Errors))
                        .WithMetadata("StatusCode", 400));
                }

                var existingUser = await _userRepository.GetUserWithDetailsAsync(userDto.Id);
                if (existingUser == null)
                {
                    return Result.Fail(new Error("User not found")
                        .WithMetadata("StatusCode", 404));
                }

                existingUser.FullName = userDto.FullName;
                existingUser.StudentNumber = userDto.StudentNumber;
                existingUser.Email = userDto.Email;
                existingUser.ContactNumber = userDto.ContactNumber;
                existingUser.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(existingUser);
                await _userRepository.SaveChangesAsync();

                return Result.Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile: {UserId}", userDto.Id);
                return Result.Fail(new Error("An error occurred while updating the user profile")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }
        public async Task<CurrentUserDto?> GetUserByIdAsync(int userId)
        {
            if (userId <= 0) return null;

            var user = await _userRepository.GetUserWithDetailsAsync(userId);
            if (user == null) return null;

            return new CurrentUserDto
            {
                Id = user.UserID,
                Username = user.Username,
                FullName = user.FullName,
                StudentNumber = user.StudentNumber ?? "",
                Email = user.Email ?? "",
                ContactNumber = user.ContactNumber ?? "",
                ProgramID = user.ProgramID,
                Program = user.Program?.Name ?? "Unknown",
                YearLevelId = user.YearLevelId,
                YearLevel = user.YearLevel?.Name ?? "Unknown",
                Role = user.Role ?? "Student",
                CreatedAt = user.DateRegistered ?? DateTime.UtcNow,
                UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
        }

        public async Task<CreateUserDto?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var user = await _userRepository.GetByUsernameAsync(username);
            return user is null ? null : MapUserToCreateDto(user);
        }

        public async Task<List<CreateUserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersWithDetailsAsync();
            return users.Select(MapUserToCreateDto).ToList();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return await _userRepository.UsernameExistsAsync(username);
        }

        public async Task<CurrentUserDto?> GetCurrentUserAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    return null;

                var user = await _userRepository.GetUserWithDetailsAsync(userId);
                if (user == null)
                    return null;

                return MapUserToCurrentUserDto(user);
            }
            catch
            {
                return null;
            }
        }

        private CreateUserDto MapUserToCreateDto(User user)
        {
            return new CreateUserDto
            {
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email ?? "",
                StudentNumber = user.StudentNumber ?? "",
                ContactNumber = user.ContactNumber ?? "",
                Password = "",
                YearLevelId = user.YearLevelId ?? 0,
                ProgramID = user.ProgramID ?? 0
            };
        }

        private CurrentUserDto MapUserToCurrentUserDto(User user)
        {
            return new CurrentUserDto
            {
                Id = user.UserID,
                Username = user.Username ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = user.Role ?? "Student",
                StudentNumber = user.StudentNumber ?? string.Empty,
                ContactNumber = user.ContactNumber ?? string.Empty,
                YearLevelId = user.YearLevelId,
                ProgramID = user.ProgramID,
                Program = user.Program?.Name ?? "Unknown",
                YearLevel = user.YearLevel?.Name ?? "Unknown",
                CreatedAt = user.DateRegistered ?? DateTime.UtcNow,
                UpdatedAt = user.UpdatedAt ?? DateTime.UtcNow,
                ProfilePictureUrl = user.ProfilePictureUrl ?? string.Empty
            };
        }
    }
}