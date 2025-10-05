using FluentResults;
using CampusHub.Application.DTO;
using CampusHub.Application.Interfaces;
using CampusHub.Application.Helpers;
using CampusHub.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CampusHub.Application.Services
{
    public class MarketplaceService : IMarketplaceService
    {
        private readonly IMarketplaceRepository _marketplaceRepository;
        private readonly ILogger<MarketplaceService> _logger;

        public MarketplaceService(
            IMarketplaceRepository marketplaceRepository,
            ILogger<MarketplaceService> logger)
        {
            _marketplaceRepository = marketplaceRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<MarketplaceItemDto>>> GetAllItemsAsync(int? currentUserId = null)
        {
            try
            {
                var items = await _marketplaceRepository.GetAllAsync();
                var itemDtos = new List<MarketplaceItemDto>();

                foreach (var item in items)
                {
                    var dto = MapToDto(item);
                    if (currentUserId.HasValue)
                    {
                        dto.IsLiked = await _marketplaceRepository.IsLikedByUserAsync(item.Id, currentUserId.Value);
                    }
                    dto.TimeAgo = GetTimeAgo(item.CreatedDate);
                    itemDtos.Add(dto);
                }

                return Result.Ok(itemDtos.OrderByDescending(x => x.CreatedDate).AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all marketplace items");
                return Result.Fail(new Error("An error occurred while retrieving marketplace items")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<MarketplaceItemDto>> GetItemByIdAsync(int id, int? currentUserId = null)
        {
            try
            {
                if (id <= 0)
                {
                    return Result.Fail(new Error("Invalid item ID: Must be greater than 0")
                        .WithMetadata("StatusCode", 400));
                }

                var item = await _marketplaceRepository.GetByIdAsync(id);
                if (item == null)
                {
                    return Result.Fail(new Error("Item not found")
                        .WithMetadata("StatusCode", 404));
                }

                var dto = MapToDto(item);
                if (currentUserId.HasValue)
                {
                    dto.IsLiked = await _marketplaceRepository.IsLikedByUserAsync(item.Id, currentUserId.Value);
                }
                dto.TimeAgo = GetTimeAgo(item.CreatedDate);

                return Result.Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving marketplace item {ItemId}", id);
                return Result.Fail(new Error("An error occurred while retrieving the item")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<MarketplaceItemDto>> CreateItemAsync(CreateMarketplaceItemDto dto)
        {
            try
            {
                var validation = ValidationHelper.ValidateCreateMarketplaceItem(dto);
                if (!validation.IsValid)
                {
                    return Result.Fail(new Error(string.Join("; ", validation.Errors))
                        .WithMetadata("StatusCode", 400));
                }

                if (dto.SellerId <= 0)
                {
                    return Result.Fail(new Error("Valid seller ID is required")
                        .WithMetadata("StatusCode", 400));
                }

                var item = new MarketplaceItem
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Price = dto.Price,
                    Category = dto.Category,
                    Condition = dto.Condition,
                    MeetupPreference = dto.MeetupPreference,
                    Location = dto.Location,
                    ImageUrl = dto.ImageUrl,
                    SellerId = dto.SellerId,
                    SellerName = dto.SellerName,
                    ContactNumber = dto.ContactNumber,
                    CreatedDate = DateTime.UtcNow,
                    Status = MarketplaceItemStatus.Active
                };

                var createdItem = await _marketplaceRepository.CreateAsync(item);
                await _marketplaceRepository.SaveChangesAsync();

                var result = MapToDto(createdItem);
                result.TimeAgo = GetTimeAgo(createdItem.CreatedDate);

                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating marketplace item");
                return Result.Fail(new Error("An error occurred while creating the marketplace item")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<MarketplaceItemDto>> UpdateItemAsync(UpdateMarketplaceItemDto dto)
        {
            try
            {
                var validation = ValidationHelper.ValidateUpdateMarketplaceItem(dto);
                if (!validation.IsValid)
                {
                    return Result.Fail(new Error(string.Join("; ", validation.Errors))
                        .WithMetadata("StatusCode", 400));
                }

                var existingItem = await _marketplaceRepository.GetByIdAsync(dto.Id);
                if (existingItem == null)
                {
                    return Result.Fail(new Error("Item not found")
                        .WithMetadata("StatusCode", 404));
                }

                existingItem.Title = dto.Title;
                existingItem.Description = dto.Description;
                existingItem.Price = dto.Price;
                existingItem.Condition = dto.Condition;
                existingItem.Category = dto.Category;
                existingItem.MeetupPreference = dto.MeetupPreference;
                existingItem.Location = dto.Location;
                existingItem.ImageUrl = dto.ImageUrl;
                existingItem.ContactNumber = dto.ContactNumber;
                existingItem.UpdatedDate = DateTime.UtcNow;

                var updatedItem = await _marketplaceRepository.UpdateAsync(existingItem);
                await _marketplaceRepository.SaveChangesAsync();

                var result = MapToDto(updatedItem);
                result.TimeAgo = GetTimeAgo(updatedItem.CreatedDate);

                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating marketplace item {ItemId}", dto.Id);
                return Result.Fail(new Error("An error occurred while updating the marketplace item")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<bool>> DeleteItemAsync(int id, int userId)
        {
            try
            {
                if (id <= 0 || userId <= 0)
                {
                    return Result.Fail(new Error("Valid ID and User ID are required")
                        .WithMetadata("StatusCode", 400));
                }

                var item = await _marketplaceRepository.GetByIdAsync(id);
                if (item == null)
                {
                    return Result.Fail(new Error("Item not found")
                        .WithMetadata("StatusCode", 404));
                }

                if (item.SellerId != userId)
                {
                    return Result.Fail(new Error("You don't have permission to delete this item")
                        .WithMetadata("StatusCode", 403));
                }

                var result = await _marketplaceRepository.DeleteAsync(id);
                await _marketplaceRepository.SaveChangesAsync();

                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting marketplace item {ItemId}", id);
                return Result.Fail(new Error("An error occurred while deleting the marketplace item")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<IEnumerable<MarketplaceItemDto>>> GetItemsByLocationAsync(CampusLocation location, int? currentUserId = null)
        {
            try
            {
                var items = await _marketplaceRepository.GetByLocationAsync(location);
                var itemDtos = new List<MarketplaceItemDto>();

                foreach (var item in items)
                {
                    var dto = MapToDto(item);
                    if (currentUserId.HasValue)
                    {
                        dto.IsLiked = await _marketplaceRepository.IsLikedByUserAsync(item.Id, currentUserId.Value);
                    }
                    dto.TimeAgo = GetTimeAgo(item.CreatedDate);
                    itemDtos.Add(dto);
                }

                return Result.Ok(itemDtos.OrderByDescending(x => x.CreatedDate).AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items by location {Location}", location);
                return Result.Fail(new Error("An error occurred while retrieving items by location")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<IEnumerable<MarketplaceItemDto>>> GetItemsBySellerAsync(int sellerId, int? currentUserId = null)
        {
            try
            {
                if (sellerId <= 0)
                {
                    return Result.Fail(new Error("Valid seller ID is required")
                        .WithMetadata("StatusCode", 400));
                }

                var items = await _marketplaceRepository.GetBySellerAsync(sellerId);
                var itemDtos = new List<MarketplaceItemDto>();

                foreach (var item in items)
                {
                    var dto = MapToDto(item);
                    if (currentUserId.HasValue)
                    {
                        dto.IsLiked = await _marketplaceRepository.IsLikedByUserAsync(item.Id, currentUserId.Value);
                    }
                    dto.TimeAgo = GetTimeAgo(item.CreatedDate);
                    itemDtos.Add(dto);
                }

                return Result.Ok(itemDtos.OrderByDescending(x => x.CreatedDate).AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items by seller {SellerId}", sellerId);
                return Result.Fail(new Error("An error occurred while retrieving seller items")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<List<MarketplaceItemDto>>> GetUserListingsAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result.Fail(new Error("Valid user ID is required")
                        .WithMetadata("StatusCode", 400));
                }

                var itemsResult = await GetItemsBySellerAsync(userId);
                if (itemsResult.IsFailed)
                {
                    return Result.Fail(itemsResult.Errors);
                }

                return Result.Ok(itemsResult.Value.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user listings for userId: {UserId}", userId);
                return Result.Fail(new Error("An error occurred while retrieving user listings")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result> UpdateItemStatusAsync(int itemId, MarketplaceItemStatus status)
        {
            try
            {
                if (itemId <= 0)
                {
                    return Result.Fail(new Error("Valid item ID is required")
                        .WithMetadata("StatusCode", 400));
                }

                var item = await _marketplaceRepository.GetByIdAsync(itemId);
                if (item == null)
                {
                    return Result.Fail(new Error("Item not found")
                        .WithMetadata("StatusCode", 404));
                }

                item.Status = status;
                item.UpdatedDate = DateTime.UtcNow;
                await _marketplaceRepository.UpdateAsync(item);
                await _marketplaceRepository.SaveChangesAsync();

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item status for {ItemId}", itemId);
                return Result.Fail(new Error("An error occurred while updating item status")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result> MarkItemAvailableAsync(ItemStatusOperationDto dto)
        {
            try
            {
                var validation = ValidationHelper.ValidateItemStatusOperation(dto);
                if (!validation.IsValid)
                {
                    return Result.Fail(new Error(string.Join("; ", validation.Errors))
                        .WithMetadata("StatusCode", 400));
                }

                return await UpdateItemStatusAsync(dto.ItemId, MarketplaceItemStatus.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking item available");
                return Result.Fail(new Error("An error occurred while marking item as available")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result> MarkItemSoldAsync(ItemStatusOperationDto dto)
        {
            try
            {
                var validation = ValidationHelper.ValidateItemStatusOperation(dto);
                if (!validation.IsValid)
                {
                    return Result.Fail(new Error(string.Join("; ", validation.Errors))
                        .WithMetadata("StatusCode", 400));
                }

                return await UpdateItemStatusAsync(dto.ItemId, MarketplaceItemStatus.Sold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking item sold");
                return Result.Fail(new Error("An error occurred while marking item as sold")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<bool>> ToggleLikeAsync(int itemId, int userId)
        {
            try
            {
                var validation = ValidationHelper.ValidateToggleLike(new ToggleLikeDto
                {
                    MarketplaceItemId = itemId,
                    UserId = userId
                });

                if (!validation.IsValid)
                {
                    return Result.Fail(new Error(string.Join("; ", validation.Errors))
                        .WithMetadata("StatusCode", 400));
                }

                var result = await _marketplaceRepository.ToggleLikeAsync(itemId, userId);
                await _marketplaceRepository.SaveChangesAsync();

                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling like for item {ItemId}", itemId);
                return Result.Fail(new Error("An error occurred while updating like status")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result> ToggleLikeAsync(ToggleLikeDto toggleDto)
        {
            try
            {
                var validation = ValidationHelper.ValidateToggleLike(toggleDto);
                if (!validation.IsValid)
                {
                    return Result.Fail(new Error(string.Join("; ", validation.Errors))
                        .WithMetadata("StatusCode", 400));
                }

                await ToggleLikeAsync(toggleDto.MarketplaceItemId, toggleDto.UserId);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling like");
                return Result.Fail(new Error("An error occurred while updating like status")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<bool>> ReportItemAsync(CreateReportDto reportDto)
        {
            try
            {
                var validation = ValidationHelper.ValidateCreateReport(reportDto);
                if (!validation.IsValid)
                {
                    return Result.Fail(new Error(string.Join("; ", validation.Errors))
                        .WithMetadata("StatusCode", 400));
                }

                var result = await _marketplaceRepository.ReportItemAsync(
                    reportDto.MarketplaceItemId,
                    reportDto.ReporterId,
                    reportDto.Reason,
                    reportDto.Description);

                await _marketplaceRepository.SaveChangesAsync();
                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reporting item");
                return Result.Fail(new Error("An error occurred while reporting the item")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<IEnumerable<ReportDto>>> GetAllReportsAsync()
        {
            try
            {
                var reports = await _marketplaceRepository.GetReportsAsync();
                return Result.Ok(reports.Select(MapToReportDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all reports");
                return Result.Fail(new Error("An error occurred while retrieving reports")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<IEnumerable<ReportDto>>> GetReportsByItemAsync(int itemId)
        {
            try
            {
                if (itemId <= 0)
                {
                    return Result.Fail(new Error("Valid item ID is required")
                        .WithMetadata("StatusCode", 400));
                }

                var reports = await _marketplaceRepository.GetReportsByItemAsync(itemId);
                return Result.Ok(reports.Select(MapToReportDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reports for item {ItemId}", itemId);
                return Result.Fail(new Error("An error occurred while retrieving item reports")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<bool>> UpdateReportStatusAsync(int reportId, ReportStatus status, int adminUserId, string? adminNotes = null)
        {
            try
            {
                if (reportId <= 0 || adminUserId <= 0)
                {
                    return Result.Fail(new Error("Valid report ID and admin user ID are required")
                        .WithMetadata("StatusCode", 400));
                }

                var result = await _marketplaceRepository.UpdateReportStatusAsync(reportId, status, adminUserId, adminNotes);
                await _marketplaceRepository.SaveChangesAsync();

                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating report status");
                return Result.Fail(new Error("An error occurred while updating report status")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<List<MarketplaceItemDto>>> GetUserItemsAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result.Fail(new Error("Valid user ID is required")
                        .WithMetadata("StatusCode", 400));
                }

                var itemsResult = await GetItemsBySellerAsync(userId);
                if (itemsResult.IsFailed)
                {
                    return Result.Fail(itemsResult.Errors);
                }

                return Result.Ok(itemsResult.Value.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user items for userId: {UserId}", userId);
                return Result.Fail(new Error("An error occurred while retrieving user items")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<List<MarketplaceItemDto>>> GetUserWishlistAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result.Fail(new Error("Valid user ID is required")
                        .WithMetadata("StatusCode", 400));
                }

                var items = await _marketplaceRepository.GetUserWishlistAsync(userId);
                var itemDtos = new List<MarketplaceItemDto>();

                foreach (var item in items)
                {
                    var dto = MapToDto(item);
                    dto.IsLiked = true;
                    dto.TimeAgo = GetTimeAgo(item.CreatedDate);
                    itemDtos.Add(dto);
                }

                return Result.Ok(itemDtos.OrderByDescending(x => x.CreatedDate).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user wishlist for userId: {UserId}", userId);
                return Result.Fail(new Error("An error occurred while retrieving wishlist")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<int>> GetUserWishlistCountAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result.Fail(new Error("Valid user ID is required")
                        .WithMetadata("StatusCode", 400));
                }

                var count = await _marketplaceRepository.GetUserWishlistCountAsync(userId);
                return Result.Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wishlist count for userId: {UserId}", userId);
                return Result.Fail(new Error("An error occurred while retrieving wishlist count")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<bool>> RemoveFromWishlistAsync(int itemId, int userId)
        {
            try
            {
                var validation = ValidationHelper.ValidateItemStatusOperation(new ItemStatusOperationDto
                {
                    ItemId = itemId,
                    UserId = userId
                });

                if (!validation.IsValid)
                {
                    return Result.Fail(new Error(string.Join("; ", validation.Errors))
                        .WithMetadata("StatusCode", 400));
                }

                var result = await _marketplaceRepository.RemoveFromWishlistAsync(itemId, userId);

                if (!result)
                {
                    return Result.Fail(new Error("Item not found in wishlist")
                        .WithMetadata("StatusCode", 404));
                }

                await _marketplaceRepository.SaveChangesAsync();
                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from wishlist");
                return Result.Fail(new Error("An error occurred while removing item from wishlist")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        public async Task<Result<UserStatsDto>> GetUserStatsAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return Result.Fail(new Error("Valid user ID is required")
                        .WithMetadata("StatusCode", 400));
                }

                var itemsResult = await GetItemsBySellerAsync(userId);
                if (itemsResult.IsFailed)
                {
                    return Result.Fail(itemsResult.Errors);
                }

                var itemsList = itemsResult.Value.ToList();

                var stats = new UserStatsDto
                {
                    TotalListings = itemsList.Count,
                    ActiveListings = itemsList.Count(x => x.Status == MarketplaceItemStatus.Active),
                    SoldItems = itemsList.Count(x => x.Status == MarketplaceItemStatus.Sold)
                };

                return Result.Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user stats for userId: {UserId}", userId);
                return Result.Fail(new Error("An error occurred while retrieving user statistics")
                    .WithMetadata("StatusCode", 500)
                    .CausedBy(ex));
            }
        }

        // Private helper methods remain the same
        private static MarketplaceItemDto MapToDto(MarketplaceItem item)
        {
            return new MarketplaceItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Price = item.Price,
                Category = item.Category,
                Condition = item.Condition,
                MeetupPreference = item.MeetupPreference,
                Location = GetLocationDisplayName(item.Location),
                ImageUrl = item.ImageUrl,
                SellerName = item.SellerName,
                SellerId = item.SellerId,
                ContactNumber = item.ContactNumber ?? string.Empty,
                LikesCount = item.LikesCount,
                CreatedDate = item.CreatedDate,
                Status = item.Status,
                UpdatedAt = item.UpdatedDate,
                CreatedAt = item.CreatedDate
            };
        }

        private static ReportDto MapToReportDto(Report report)
        {
            return new ReportDto
            {
                Id = report.Id,
                MarketplaceItemId = report.MarketplaceItemId,
                ReporterId = report.ReporterUserID,
                Reason = report.Reason,
                Description = report.Description,
                Status = report.Status.ToString(),
                CreatedAt = report.CreatedAt,
                AdminNotes = report.AdminNotes,
                AdminUserId = report.ResolvedByUserId
            };
        }

        private static string GetLocationDisplayName(CampusLocation location)
        {
            return location switch
            {
                CampusLocation.MainCampus => "Main Campus",
                CampusLocation.Congressional => "Congressional Extension Campus",
                CampusLocation.BagongSilang => "Bagong Silang Extension Campus",
                CampusLocation.Camarin => "Camarin Extension Campus",
                _ => "Unknown"
            };
        }

        private static string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.UtcNow - date;

            return timeSpan.TotalDays switch
            {
                < 1 => timeSpan.TotalHours < 1 ? "Just now" : $"{(int)timeSpan.TotalHours} hours ago",
                < 7 => $"{(int)timeSpan.TotalDays} days ago",
                < 30 => $"{(int)(timeSpan.TotalDays / 7)} weeks ago",
                _ => $"{(int)(timeSpan.TotalDays / 30)} months ago"
            };
        }
    }
}