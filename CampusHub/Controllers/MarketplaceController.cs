using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using FluentResults.Extensions.AspNetCore;
using CampusHub.Application.DTO.Marketplace;
using CampusHub.Application.DTO.User;
using CampusHub.Application.DTO.Common;

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
            var result = await _marketplaceService.GetAllItemsAsync(userId);
            return result.ToActionResult();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<MarketplaceItemDto>> GetItemById(int id, [FromQuery] int? userId = null)
        {
            var result = await _marketplaceService.GetItemByIdAsync(id, userId);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<MarketplaceItemDto>> CreateItem([FromBody] CreateMarketplaceItemDto createItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _marketplaceService.CreateItemAsync(createItemDto);
            return result.ToActionResult();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MarketplaceItemDto>> UpdateItem(int id, [FromBody] UpdateMarketplaceItemDto updateItemDto)
        {
            if (id != updateItemDto.Id)
            {
                return BadRequest(new { message = "ID in URL does not match ID in request body" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _marketplaceService.UpdateItemAsync(updateItemDto);
            return result.ToActionResult();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteItem(int id, [FromQuery] int userId)
        {
            var result = await _marketplaceService.DeleteItemAsync(id, userId);
            return result.ToActionResult();
        }

        [HttpPost("{id}/toggle-like")]
        public async Task<ActionResult<bool>> ToggleLike(int id, [FromBody] ToggleLikeDto toggleLikeDto)
        {
            if (id != toggleLikeDto.ItemId)
            {
                return BadRequest(new { message = "Item ID in URL does not match ID in request body" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _marketplaceService.ToggleLikeAsync(toggleLikeDto.ItemId, toggleLikeDto.UserId);
            return result.ToActionResult();
        }

        [HttpGet("location/{location}")]
        public async Task<ActionResult<IEnumerable<MarketplaceItemDto>>> GetItemsByLocation(CampusLocation location, [FromQuery] int? userId = null)
        {
            var result = await _marketplaceService.GetItemsByLocationAsync(location, userId);
            return result.ToActionResult();
        }

        [HttpGet("seller/{sellerId}")]
        public async Task<ActionResult<IEnumerable<MarketplaceItemDto>>> GetItemsBySeller(int sellerId, [FromQuery] int? userId = null)
        {
            var result = await _marketplaceService.GetItemsBySellerAsync(sellerId, userId);
            return result.ToActionResult();
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<MarketplaceItemDto>>> GetItemsByCategory(ItemCategory category, [FromQuery] int? userId = null)
        {
            var allItemsResult = await _marketplaceService.GetAllItemsAsync(userId);

            if (allItemsResult.IsFailed)
            {
                return allItemsResult.ToActionResult();
            }

            var filteredItems = allItemsResult.Value.Where(i => i.Category == category);
            return Ok(filteredItems);
        }

        [HttpGet("wishlist/{userId}")]
        public async Task<ActionResult<IEnumerable<MarketplaceItemDto>>> GetUserWishlist(int userId)
        {
            var result = await _marketplaceService.GetUserWishlistAsync(userId);
            return result.ToActionResult();
        }

        [HttpGet("wishlist/{userId}/count")]
        public async Task<ActionResult<int>> GetUserWishlistCount(int userId)
        {
            var result = await _marketplaceService.GetUserWishlistCountAsync(userId);
            return result.ToActionResult();
        }

        [HttpDelete("wishlist/{itemId}/user/{userId}")]
        public async Task<ActionResult<bool>> RemoveFromWishlist(int itemId, int userId)
        {
            var result = await _marketplaceService.RemoveFromWishlistAsync(itemId, userId);
            return result.ToActionResult();
        }

        [HttpPost("{itemId}/report")]
        public async Task<ActionResult<bool>> ReportItem(int itemId, [FromBody] CreateReportDto reportDto)
        {
            reportDto.MarketplaceItemId = itemId;
            var result = await _marketplaceService.ReportItemAsync(reportDto);
            return result.ToActionResult();
        }

        [HttpGet("reports")]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetAllReports()
        {
            var result = await _marketplaceService.GetAllReportsAsync();
            return result.ToActionResult();
        }

        [HttpGet("{itemId}/reports")]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetReportsByItem(int itemId)
        {
            var result = await _marketplaceService.GetReportsByItemAsync(itemId);
            return result.ToActionResult();
        }

        [HttpPut("reports/{reportId}/status")]
        public async Task<ActionResult<bool>> UpdateReportStatus(int reportId, [FromBody] UpdateReportStatusDto dto)
        {
            var result = await _marketplaceService.UpdateReportStatusAsync(
                reportId,
                dto.Status,
                dto.AdminUserId,
                dto.AdminNotes);

            return result.ToActionResult();
        }

        [HttpPut("{itemId}/mark-available")]
        public async Task<ActionResult> MarkItemAvailable(int itemId, [FromBody] ItemStatusOperationDto dto)
        {
            dto.ItemId = itemId;
            var result = await _marketplaceService.MarkItemAvailableAsync(dto);
            return result.ToActionResult();
        }

        [HttpPut("{itemId}/mark-sold")]
        public async Task<ActionResult> MarkItemSold(int itemId, [FromBody] ItemStatusOperationDto dto)
        {
            dto.ItemId = itemId;
            var result = await _marketplaceService.MarkItemSoldAsync(dto);
            return result.ToActionResult();
        }

        [HttpGet("stats/{userId}")]
        public async Task<ActionResult<UserStatsDto>> GetUserStats(int userId)
        {
            var result = await _marketplaceService.GetUserStatsAsync(userId);
            return result.ToActionResult();
        }

        [HttpGet("user/{userId}/items")]
        public async Task<ActionResult<List<MarketplaceItemDto>>> GetUserItems(int userId)
        {
            var result = await _marketplaceService.GetUserItemsAsync(userId);
            return result.ToActionResult();
        }

        [HttpGet("user/{userId}/listings")]
        public async Task<ActionResult<List<MarketplaceItemDto>>> GetUserListings(int userId)
        {
            var result = await _marketplaceService.GetUserListingsAsync(userId);
            return result.ToActionResult();
        }
    }
}