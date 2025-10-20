using CampusHub.Application.DTO.Common;
using CampusHub.Application.DTO.User;
using CampusHub.Application.Interfaces;
using FluentResults.Extensions.AspNetCore;
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Login attempt for username: {Username}", loginDto.Username);

            var result = await _userService.LoginAsync(loginDto);

            if (result.IsFailed)
            {
                _logger.LogWarning("Failed login attempt for username: {Username}", loginDto.Username);
                return result.ToActionResult();
            }

            var loginResponse = result.Value;
            _logger.LogInformation("Login successful for user: {Username}, Role: {Role}",
                loginResponse.Username, loginResponse.Role);

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

            _logger.LogInformation("Authentication cookie set for user: {Username}", loginResponse.Username);
            _logger.LogInformation("Redirect URL: {RedirectUrl}", loginResponse.RedirectUrl);

            return Ok(loginResponse);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out successfully");

            var result = await _userService.LogoutAsync();
            return result.ToActionResult();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Registration attempt for username: {Username}", createUserDto.Username);

            var result = await _userService.CreateUserAsync(createUserDto);

            if (result.IsSuccess)
            {
                _logger.LogInformation("User registered successfully: {Username}", createUserDto.Username);
            }

            return result.ToActionResult();
        }
    }
}