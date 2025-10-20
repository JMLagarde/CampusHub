using CampusHub.Application.DTO.User;
using CampusHub.Application.Interfaces;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusHub.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMarketplaceService _marketplaceService;

        public UserController(IUserService userService, IMarketplaceService marketplaceService)
        {
            _userService = userService;
            _marketplaceService = marketplaceService;
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            if (userDto.Id != userId.Value)
            {
                return Forbid();
            }

            var result = await _userService.UpdateUserProfileAsync(userDto);
            return result.ToActionResult();
        }

        [HttpGet("listings")]
        public async Task<IActionResult> GetUserListings()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var result = await _marketplaceService.GetUserItemsAsync(userId.Value);
            return result.ToActionResult();
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetUserStats()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var result = await _marketplaceService.GetUserStatsAsync(userId.Value);
            return result.ToActionResult();
        }

        [HttpGet("wishlist")]
        public async Task<IActionResult> GetUserWishlist()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var result = await _marketplaceService.GetUserWishlistAsync(userId.Value);
            return result.ToActionResult();
        }

        [HttpGet("wishlist/count")]
        public async Task<IActionResult> GetUserWishlistCount()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var result = await _marketplaceService.GetUserWishlistCountAsync(userId.Value);
            return result.ToActionResult();
        }

        [HttpDelete("wishlist/item/{itemId}")]
        public async Task<IActionResult> RemoveFromWishlist(int itemId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var result = await _marketplaceService.RemoveFromWishlistAsync(itemId, userId.Value);
            return result.ToActionResult();
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}