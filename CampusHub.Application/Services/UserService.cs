using CampusHub.Application.DTO;
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
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentException("Username is required");
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ArgumentException("Full name is required");
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required");

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

            return await _userRepository.AddAsync(user);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username))
                throw new ArgumentException("Username is required");
            if (string.IsNullOrWhiteSpace(loginDto.Password))
                throw new ArgumentException("Password is required");

            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null || user.Password != loginDto.Password)
                throw new UnauthorizedAccessException("Invalid username or password");

            return new LoginResponseDto
            {
                UserId = user.UserID,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role ?? string.Empty,
                Email = user.Email,
                IsSuccess = true,
                Message = "Login successful"
            };
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
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

        public async Task<CurrentUserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserWithDetailsAsync(id);
            return user is null ? null : MapUserToCurrentUserDto(user);
        }

        public async Task<CreateUserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user is null ? null : MapUserToDto(user);
        }

        public async Task<List<CreateUserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersWithDetailsAsync();
            return users.Select(MapUserToDto).ToList();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _userRepository.UsernameExistsAsync(username);
        }

        public async Task<CurrentUserDto?> GetCurrentUserAsync()
        {
            throw new NotImplementedException("Use GetCurrentUserAsync(int userId) instead, after getting userId from sessionStorage in your component");
        }
        public async Task<CurrentUserDto?> GetCurrentUserAsync(int userId)
        {
            try
            {
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
        public Task<CreateUserDto?> GetCurrentUserAsync(string sessionToken)
        {
            throw new NotImplementedException();
        }

        private CreateUserDto MapUserToDto(User user)
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
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role ?? "Student",
                StudentNumber = user.StudentNumber,
                ContactNumber = user.ContactNumber,
                YearLevelId = user.YearLevelId,
                ProgramID = user.ProgramID
            };
        }

        Task<CreateUserDto?> IUserService.GetUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}