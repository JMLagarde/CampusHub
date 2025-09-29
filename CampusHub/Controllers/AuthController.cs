using CampusHub.Application.DTO;
using CampusHub.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusHub.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation($"Login attempt for username: {loginDto.Username}");

                var loginResponse = await _userService.LoginAsync(loginDto);

                _logger.LogInformation($"Login successful for user: {loginResponse.Username}, Role: {loginResponse.Role}");

                // Create claims for the authenticated user
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, loginResponse.UserId.ToString()),
                    new(ClaimTypes.Name, loginResponse.Username),
                    new(ClaimTypes.Role, loginResponse.Role),
                    new("FullName", loginResponse.FullName)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
                    });

                _logger.LogInformation($"Authentication cookie set for user: {loginResponse.Username}");
                _logger.LogInformation($"Redirect URL: {loginResponse.RedirectUrl}");

                // Return the structure that matches LoginResponseDto
                return Ok(loginResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Unauthorized login attempt for username: {loginDto.Username}");
                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Argument error during login: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error during login for username: {loginDto.Username}");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                _logger.LogInformation("User logged out successfully");
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var userIdResult = await _userService.CreateUserAsync(createUserDto);
                _logger.LogInformation($"User registered successfully: {createUserDto.Username}");

                return Ok(new { message = "User registered successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Registration conflict for username: {createUserDto.Username} - {ex.Message}");
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Registration validation error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error during registration for username: {createUserDto.Username}");
                return StatusCode(500, new { message = "An internal error occurred" });
            }
        }
    }
}