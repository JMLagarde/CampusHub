using CampusHub.Application.DTO;
using CampusHub.Application.Interfaces;
using CampusHub.Application.Helpers;
using CampusHub.Domain.Entities;

namespace CampusHub.Application.Services
{
    public class MarketplaceService : IMarketplaceService
    {
        private readonly IMarketplaceRepository _marketplaceRepository;

        public MarketplaceService(IMarketplaceRepository marketplaceRepository)
        {
            _marketplaceRepository = marketplaceRepository;
        }

        #region Item CRUD Operations

        public async Task<IEnumerable<MarketplaceItemDto>> GetAllItemsAsync(int? currentUserId = null)
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

            return itemDtos.OrderByDescending(x => x.CreatedDate);
        }

        public async Task<MarketplaceItemDto?> GetItemByIdAsync(int id, int? currentUserId = null)
        {
            if (id <= 0)
                throw new ArgumentException("Valid item ID is required");

            var item = await _marketplaceRepository.GetByIdAsync(id);
            if (item == null) return null;

            var dto = MapToDto(item);
            if (currentUserId.HasValue)
            {
                dto.IsLiked = await _marketplaceRepository.IsLikedByUserAsync(item.Id, currentUserId.Value);
            }
            dto.TimeAgo = GetTimeAgo(item.CreatedDate);

            return dto;
        }

        public async Task<MarketplaceItemDto> CreateItemAsync(CreateMarketplaceItemDto dto)
        {
            // Validate using ValidationHelper
            var validation = ValidationHelper.ValidateCreateMarketplaceItem(dto);
            if (!validation.IsValid)
            {
                throw new ArgumentException(string.Join("; ", validation.Errors));
            }

            // Business logic validations
            if (dto.SellerId <= 0)
            {
                throw new ArgumentException("Valid seller ID is required");
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

            return result;
        }

        public async Task<MarketplaceItemDto> UpdateItemAsync(UpdateMarketplaceItemDto dto)
        {
            // Validate using ValidationHelper
            var validation = ValidationHelper.ValidateUpdateMarketplaceItem(dto);
            if (!validation.IsValid)
            {
                throw new ArgumentException(string.Join("; ", validation.Errors));
            }

            // Business logic validations
            var existingItem = await _marketplaceRepository.GetByIdAsync(dto.Id);
            if (existingItem == null)
                throw new ArgumentException("Item not found");

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

            return result;
        }

        public async Task<bool> DeleteItemAsync(int id, int userId)
        {
            if (id <= 0 || userId <= 0)
                throw new ArgumentException("Valid ID and User ID are required");

            var item = await _marketplaceRepository.GetByIdAsync(id);
            if (item == null || item.SellerId != userId)
                return false;

            var result = await _marketplaceRepository.DeleteAsync(id);
            await _marketplaceRepository.SaveChangesAsync();

            return result;
        }

        #endregion

        #region Item Queries

        public async Task<IEnumerable<MarketplaceItemDto>> GetItemsByLocationAsync(CampusLocation location, int? currentUserId = null)
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

            return itemDtos.OrderByDescending(x => x.CreatedDate);
        }

        public async Task<IEnumerable<MarketplaceItemDto>> GetItemsBySellerAsync(int sellerId, int? currentUserId = null)
        {
            if (sellerId <= 0)
                throw new ArgumentException("Valid seller ID is required");

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

            return itemDtos.OrderByDescending(x => x.CreatedDate);
        }

        public async Task<List<MarketplaceItemDto>> GetUserListingsAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Valid user ID is required");

            var items = await GetItemsBySellerAsync(userId);
            return items.ToList();
        }

        public async Task<List<MarketplaceItemDto>> GetUserItemsAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Valid user ID is required");

            var items = await GetItemsBySellerAsync(userId);
            return items.ToList();
        }

        #endregion

        #region Item Status Management

        public async Task UpdateItemStatusAsync(int itemId, MarketplaceItemStatus status)
        {
            if (itemId <= 0)
                throw new ArgumentException("Valid item ID is required");

            var item = await _marketplaceRepository.GetByIdAsync(itemId);
            if (item != null)
            {
                item.Status = status;
                item.UpdatedDate = DateTime.UtcNow;
                await _marketplaceRepository.UpdateAsync(item);
                await _marketplaceRepository.SaveChangesAsync();
            }
        }

        public async Task MarkItemAvailableAsync(ItemStatusOperationDto dto)
        {
            var validation = ValidationHelper.ValidateItemStatusOperation(dto);
            if (!validation.IsValid)
                throw new ArgumentException(string.Join("; ", validation.Errors));

            await UpdateItemStatusAsync(dto.ItemId, MarketplaceItemStatus.Active);
        }

        public async Task MarkItemSoldAsync(ItemStatusOperationDto dto)
        {
            var validation = ValidationHelper.ValidateItemStatusOperation(dto);
            if (!validation.IsValid)
                throw new ArgumentException(string.Join("; ", validation.Errors));

            await UpdateItemStatusAsync(dto.ItemId, MarketplaceItemStatus.Sold);
        }

        #endregion

        #region Like/Wishlist Operations

        public async Task<bool> ToggleLikeAsync(int itemId, int userId)
        {
            var validation = ValidationHelper.ValidateToggleLike(new ToggleLikeDto
            {
                MarketplaceItemId = itemId,
                UserId = userId
            });

            if (!validation.IsValid)
                throw new ArgumentException(string.Join("; ", validation.Errors));

            var result = await _marketplaceRepository.ToggleLikeAsync(itemId, userId);
            await _marketplaceRepository.SaveChangesAsync();

            return result;
        }

        public async Task ToggleLikeAsync(ToggleLikeDto toggleDto)
        {
            var validation = ValidationHelper.ValidateToggleLike(toggleDto);
            if (!validation.IsValid)
                throw new ArgumentException(string.Join("; ", validation.Errors));

            await ToggleLikeAsync(toggleDto.MarketplaceItemId, toggleDto.UserId);
        }

        public async Task<List<MarketplaceItemDto>> GetUserWishlistAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Valid user ID is required");

            var items = await _marketplaceRepository.GetUserWishlistAsync(userId);
            var itemDtos = new List<MarketplaceItemDto>();

            foreach (var item in items)
            {
                var dto = MapToDto(item);
                dto.IsLiked = true;
                dto.TimeAgo = GetTimeAgo(item.CreatedDate);
                itemDtos.Add(dto);
            }

            return itemDtos.OrderByDescending(x => x.CreatedDate).ToList();
        }

        public async Task<int> GetUserWishlistCountAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Valid user ID is required");

            return await _marketplaceRepository.GetUserWishlistCountAsync(userId);
        }

        public async Task<bool> RemoveFromWishlistAsync(int itemId, int userId)
        {
            var validation = ValidationHelper.ValidateItemStatusOperation(new ItemStatusOperationDto
            {
                ItemId = itemId,
                UserId = userId
            });

            if (!validation.IsValid)
                throw new ArgumentException(string.Join("; ", validation.Errors));

            var result = await _marketplaceRepository.RemoveFromWishlistAsync(itemId, userId);
            await _marketplaceRepository.SaveChangesAsync();

            return result;
        }

        #endregion

        #region Reporting Operations

        public async Task<bool> ReportItemAsync(CreateReportDto reportDto)
        {
            var validation = ValidationHelper.ValidateCreateReport(reportDto);
            if (!validation.IsValid)
                throw new ArgumentException(string.Join("; ", validation.Errors));

            var result = await _marketplaceRepository.ReportItemAsync(
                reportDto.MarketplaceItemId,  // Using your original naming convention
                reportDto.ReporterId,
                reportDto.Reason,
                reportDto.Description);

            await _marketplaceRepository.SaveChangesAsync();
            return result;
        }

        public async Task<IEnumerable<ReportDto>> GetAllReportsAsync()
        {
            var reports = await _marketplaceRepository.GetReportsAsync();
            return reports.Select(MapToReportDto);
        }

        public async Task<IEnumerable<ReportDto>> GetReportsByItemAsync(int itemId)
        {
            if (itemId <= 0)
                throw new ArgumentException("Valid item ID is required");

            var reports = await _marketplaceRepository.GetReportsByItemAsync(itemId);
            return reports.Select(MapToReportDto);
        }

        public async Task<bool> UpdateReportStatusAsync(int reportId, ReportStatus status, int adminUserId, string? adminNotes = null)
        {
            if (reportId <= 0 || adminUserId <= 0)
                throw new ArgumentException("Valid report ID and admin user ID are required");

            var result = await _marketplaceRepository.UpdateReportStatusAsync(reportId, status, adminUserId, adminNotes);
            await _marketplaceRepository.SaveChangesAsync();
            return result;
        }

        #endregion

        #region Statistics

        public async Task<UserStatsDto> GetUserStatsAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Valid user ID is required");

            var userItems = await GetItemsBySellerAsync(userId);
            var itemsList = userItems.ToList();

            return new UserStatsDto
            {
                TotalListings = itemsList.Count,
                ActiveListings = itemsList.Count(x => x.Status == MarketplaceItemStatus.Active),
                SoldItems = itemsList.Count(x => x.Status == MarketplaceItemStatus.Sold)
            };
        }

        #endregion

        #region Private Helper Methods

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

        #endregion
    }
}