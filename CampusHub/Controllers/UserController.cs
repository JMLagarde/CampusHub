using CampusHub.Application.DTO;
using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
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
        public async Task<ActionResult> UpdateUserProfile([FromBody] UpdateUserProfileDto userDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized("User not found");
                }

                if (userDto.Id != userId.Value)
                {
                    return Forbid("You can only update your own profile");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _userService.UpdateUserProfileAsync(userDto);
                if (success)
                {
                    return Ok(new { message = "Profile updated successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Failed to update profile" });
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("listings")]
        public async Task<ActionResult<List<MarketplaceItemDto>>> GetUserListings()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized("User not found");
                }

                var listings = await _marketplaceService.GetUserItemsAsync(userId.Value);
                return Ok(listings);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<UserStatsDto>> GetUserStats()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized("User not found");
                }

                var stats = await _marketplaceService.GetUserStatsAsync(userId.Value);
                return Ok(stats);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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

        [HttpGet("wishlist")]
        public async Task<ActionResult<List<MarketplaceItemDto>>> GetUserWishlist()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized("User not found");
                }

                var wishlistItems = await _marketplaceService.GetUserWishlistAsync(userId.Value);
                return Ok(wishlistItems);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("wishlist/count")]
        public async Task<ActionResult<int>> GetUserWishlistCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized("User not found");
                }

                var count = await _marketplaceService.GetUserWishlistCountAsync(userId.Value);
                return Ok(count);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("wishlist/item/{itemId}")]
        public async Task<IActionResult> RemoveFromWishlist(int itemId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized("User not found");
                }

                var success = await _marketplaceService.RemoveFromWishlistAsync(itemId, userId.Value);

                if (!success)
                {
                    return NotFound(new { message = "Item not found in wishlist" });
                }

                return Ok(new { message = "Item removed from wishlist successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}