using CampusHub.Application.DTO;
using CampusHub.Application.Helpers;
using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;

namespace CampusHub.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> CreateUserAsync(CreateUserDto dto)
        {
            // STEP 1: Validate using ValidationHelper
            var validation = ValidationHelper.ValidateCreateUser(dto);
            if (!validation.IsValid)
            {
                throw new ArgumentException(string.Join("; ", validation.Errors));
            }

            // STEP 2: Business logic validations
            if (await _userRepository.UsernameExistsAsync(dto.Username))
                throw new InvalidOperationException("Username already exists");

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

            return result;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            // STEP 1: Validate using ValidationHelper
            var validation = ValidationHelper.ValidateLogin(loginDto);
            if (!validation.IsValid)
            {
                throw new ArgumentException(string.Join("; ", validation.Errors));
            }

            // STEP 2: Business logic validations
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null || user.Password != loginDto.Password)
                throw new UnauthorizedAccessException("Invalid username or password");

            // Determine redirect URL based on role
            string redirectUrl = user.IsAdmin() ? "/admin/events" : "/main-marketplace";

            return new LoginResponseDto
            {
                UserId = user.UserID,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role ?? "Student",
                Email = user.Email,
                IsSuccess = true,
                Message = "Login successful",
                RedirectUrl = redirectUrl // Add this property to your LoginResponseDto
            };
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var validation = ValidationHelper.ValidateLogin(new LoginDto
            {
                Username = username,
                Password = password
            });

            if (!validation.IsValid)
                return false;

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                return false;

            return user.Password == password;
        }

        public Task LogoutAsync()
        {
            return Task.CompletedTask;
        }

        public async Task<CurrentUserDto?> GetUserByIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Valid user ID is required");

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

        public async Task<bool> UpdateUserProfileAsync(UpdateUserProfileDto userDto)
        {
            var validation = ValidationHelper.ValidateUpdateUserProfile(userDto);
            if (!validation.IsValid)
            {
                throw new ArgumentException(string.Join("; ", validation.Errors));
            }

            try
            {
                var existingUser = await _userRepository.GetUserWithDetailsAsync(userDto.Id);
                if (existingUser == null)
                    return false;

                existingUser.FullName = userDto.FullName;
                existingUser.StudentNumber = userDto.StudentNumber;
                existingUser.Email = userDto.Email;
                existingUser.ContactNumber = userDto.ContactNumber;
                existingUser.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(existingUser);
                await _userRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user profile: {ex.Message}");
                return false;
            }
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