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

        /// <summary>
        /// Get all marketplace items with optional user context for like status
        /// </summary>
        /// <param name="userId">Optional user ID to check like status</param>
        /// <returns>List of marketplace items</returns>
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

        /// <summary>
        /// Get a specific marketplace item by ID
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <param name="userId">Optional user ID to check like status</param>
        /// <returns>Marketplace item details</returns>
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

        /// <summary>
        /// Create a new marketplace item
        /// </summary>
        /// <param name="createItemDto">Item creation data</param>
        /// <returns>Created marketplace item</returns>
        [HttpPost]
        public async Task<ActionResult<MarketplaceItemDto>> CreateItem([FromBody] CreateMarketplaceItemDto createItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdItem = await _marketplaceService.CreateItemAsync(createItemDto);
                return CreatedAtAction(nameof(GetItemById), new { id = createdItem.Id }, createdItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the marketplace item", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing marketplace item
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <param name="updateItemDto">Item update data</param>
        /// <returns>Updated marketplace item</returns>
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

        /// <summary>
        /// Delete a marketplace item
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <param name="userId">User ID of the item owner</param>
        /// <returns>Success status</returns>
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

        /// <summary>
        /// Toggle like status for a marketplace item
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <param name="toggleLikeDto">Like toggle data</param>
        /// <returns>Success status</returns>
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

        /// <summary>
        /// Get marketplace items by campus location
        /// </summary>
        /// <param name="location">Campus location</param>
        /// <param name="userId">Optional user ID to check like status</param>
        /// <returns>Filtered marketplace items</returns>
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

        /// <summary>
        /// Get marketplace items by seller
        /// </summary>
        /// <param name="sellerId">Seller user ID</param>
        /// <param name="userId">Optional current user ID to check like status</param>
        /// <returns>Seller's marketplace items</returns>
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

        /// <summary>
        /// Get marketplace items by category
        /// </summary>
        /// <param name="category">Item category</param>
        /// <param name="userId">Optional user ID to check like status</param>
        /// <returns>Filtered marketplace items by category</returns>
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

        /// <summary>
        /// Search marketplace items by title or description
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="userId">Optional user ID to check like status</param>
        /// <returns>Search results</returns>
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
    }
}
