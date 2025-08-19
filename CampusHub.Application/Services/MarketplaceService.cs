using CampusHub.Application.DTO;
using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Application.Services
{
    public class MarketplaceService : IMarketplaceService
    {
        private readonly IMarketplaceRepository _marketplaceRepository;
        //apdbcontext palipat dito

        public MarketplaceService(IMarketplaceRepository marketplaceRepository)
        {
            _marketplaceRepository = marketplaceRepository;
        }

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
                CreatedDate = DateTime.UtcNow
            };

            var createdItem = await _marketplaceRepository.CreateAsync(item);
            var result = MapToDto(createdItem);
            result.TimeAgo = GetTimeAgo(createdItem.CreatedDate);

            return result;
        }

        public async Task<MarketplaceItemDto> UpdateItemAsync(UpdateMarketplaceItemDto dto)
        {
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
            existingItem.UpdatedDate = DateTime.UtcNow;

            var updatedItem = await _marketplaceRepository.UpdateAsync(existingItem);
            var result = MapToDto(updatedItem);
            result.TimeAgo = GetTimeAgo(updatedItem.CreatedDate);

            return result;
        }

        public async Task<bool> DeleteItemAsync(int id, int userId)
        {
            var item = await _marketplaceRepository.GetByIdAsync(id);
            if (item == null || item.SellerId != userId)
                return false;

            return await _marketplaceRepository.DeleteAsync(id);
        }

        public async Task<bool> ToggleLikeAsync(int itemId, int userId)
        {
            return await _marketplaceRepository.ToggleLikeAsync(itemId, userId);
        }

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
                LikesCount = item.LikesCount,
                CreatedDate = item.CreatedDate
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