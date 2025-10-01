using CampusHub.Application.DTO;
using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CampusHub.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketplaceController : Controller
    {
        private readonly IMarketplaceService _marketplaceService;
        private readonly IUserService _userService;

        public MarketplaceController(IMarketplaceService marketplaceService, IUserService userService)
        {
            _marketplaceService = marketplaceService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarketplaceItemDto>>> GetAllItems([FromQuery] int? userId = null)
        {
            try
            {
                var items = await _marketplaceService.GetAllItemsAsync(userId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving marketplace items", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MarketplaceItemDto>> GetItemById(int id, [FromQuery] int? userId = null)
        {
            try
            {
                var item = await _marketplaceService.GetItemByIdAsync(id, userId);

                if (item == null)
                {
                    return NotFound(new { message = "Marketplace item not found" });
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the marketplace item", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<MarketplaceItemDto>> CreateItem([FromBody] CreateMarketplaceItemDto createItemDto)
        {
            try //move try catch to service
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdItem = await _marketplaceService.CreateItemAsync(createItemDto); //create result
               // return createdItemResult.ToActionResult();
               return CreatedAtAction(nameof(GetItemById), new { id = createdItem.Id }, createdItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the marketplace item", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MarketplaceItemDto>> UpdateItem(int id, [FromBody] UpdateMarketplaceItemDto updateItemDto)
        {
            try
            {
                if (id != updateItemDto.Id)
                {
                    return BadRequest(new { message = "ID in URL does not match ID in request body" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedItem = await _marketplaceService.UpdateItemAsync(updateItemDto);
                return Ok(updatedItem);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the marketplace item", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id, [FromQuery] int userId)
        {
            try
            {
                var success = await _marketplaceService.DeleteItemAsync(id, userId);

                if (!success)
                {
                    return NotFound(new { message = "Item not found or you don't have permission to delete it" });
                }

                return Ok(new { message = "Item deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the marketplace item", error = ex.Message });
            }
        }

        [HttpPost("{id}/toggle-like")]
        public async Task<IActionResult> ToggleLike(int id, [FromBody] ToggleLikeDto toggleLikeDto)
        {
            try
            {
                if (id != toggleLikeDto.ItemId)
                {
                    return BadRequest(new { message = "Item ID in URL does not match ID in request body" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _marketplaceService.ToggleLikeAsync(toggleLikeDto.ItemId, toggleLikeDto.UserId);

                if (!success)
                {
                    return BadRequest(new { message = "Failed to toggle like status" });
                }

                return Ok(new { message = "Like status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating like status", error = ex.Message });
            }
        }

        [HttpGet("location/{location}")]
        public async Task<ActionResult<IEnumerable<MarketplaceItemDto>>> GetItemsByLocation(CampusLocation location, [FromQuery] int? userId = null)
        {
            try
            {
                var items = await _marketplaceService.GetItemsByLocationAsync(location, userId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving marketplace items by location", error = ex.Message });
            }
        }

        [HttpGet("seller/{sellerId}")]
        public async Task<ActionResult<IEnumerable<MarketplaceItemDto>>> GetItemsBySeller(int sellerId, [FromQuery] int? userId = null)
        {
            try
            {
                var items = await _marketplaceService.GetItemsBySellerAsync(sellerId, userId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving seller's marketplace items", error = ex.Message });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<MarketplaceItemDto>>> GetItemsByCategory(ItemCategory category, [FromQuery] int? userId = null)
        {
            try
            {
                var items = await _marketplaceService.GetAllItemsAsync(userId);
                var filteredItems = items.Where(i => i.Category == category);
                return Ok(filteredItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving marketplace items by category", error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MarketplaceItemDto>>> SearchItems([FromQuery] string searchTerm, [FromQuery] int? userId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Search term is required" });
                }

                var items = await _marketplaceService.GetAllItemsAsync(userId);
                var searchResults = items.Where(i =>
                    i.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    i.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).OrderByDescending(x => x.CreatedDate);

                return Ok(searchResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching marketplace items", error = ex.Message });
            }
        }

        [HttpGet("wishlist/{userId}")]
        public async Task<ActionResult<IEnumerable<MarketplaceItemDto>>> GetUserWishlist(int userId)
        {
            try
            {
                var wishlistItems = await _marketplaceService.GetUserWishlistAsync(userId);
                return Ok(wishlistItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user wishlist", error = ex.Message });
            }
        }

        [HttpGet("wishlist/{userId}/count")]
        public async Task<ActionResult<int>> GetUserWishlistCount(int userId)
        {
            try
            {
                var count = await _marketplaceService.GetUserWishlistCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving wishlist count", error = ex.Message });
            }
        }

        [HttpDelete("wishlist/{itemId}/user/{userId}")]
        public async Task<IActionResult> RemoveFromWishlist(int itemId, int userId)
        {
            try
            {
                var success = await _marketplaceService.RemoveFromWishlistAsync(itemId, userId);

                if (!success)
                {
                    return NotFound(new { message = "Item not found in wishlist" });
                }

                return Ok(new { message = "Item removed from wishlist successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing item from wishlist", error = ex.Message });
            }
        }
        [HttpPost("{itemId}/report")]
        public async Task<IActionResult> ReportItem(int itemId, [FromBody] CreateReportDto reportDto)
        {
            try
            {
                reportDto.MarketplaceItemId = itemId;
                var success = await _marketplaceService.ReportItemAsync(reportDto);

                if (success)
                {
                    return Ok(new { message = "Item reported successfully" });
                }

                return BadRequest(new { message = "Failed to report item" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("reports")]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetAllReports()
        {
            try
            {
                var reports = await _marketplaceService.GetAllReportsAsync();
                return Ok(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{itemId}/reports")]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetReportsByItem(int itemId)
        {
            try
            {
                var reports = await _marketplaceService.GetReportsByItemAsync(itemId);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("reports/{reportId}/status")]
        public async Task<IActionResult> UpdateReportStatus(int reportId, [FromBody] UpdateReportStatusDto dto)
        {
            try
            {
                var success = await _marketplaceService.UpdateReportStatusAsync(
                    reportId,
                    dto.Status,
                    dto.AdminUserId,
                    dto.AdminNotes);

                if (success)
                {
                    return Ok(new { message = "Report status updated successfully" });
                }

                return NotFound(new { message = "Report not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}