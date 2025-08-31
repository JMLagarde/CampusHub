using CampusHub.Application.DTO;
using CampusHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusHub.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _userService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the full exception details
                Console.WriteLine($"Login error: {ex}");
                return StatusCode(500, new { message = "An internal error occurred", details = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var userIdResult = await _userService.CreateUserAsync(createUserDto);
                
                return Ok(new { message = "User registered successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An internal error occurred" });
            }
        }
    }
}