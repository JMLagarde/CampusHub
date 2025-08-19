using CampusHub.Application.DTO;
using CampusHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Application.Interfaces
{
    public interface IMarketplaceService
    {
        Task<IEnumerable<MarketplaceItemDto>> GetAllItemsAsync(int? currentUserId = null);
        Task<MarketplaceItemDto?> GetItemByIdAsync(int id, int? currentUserId = null);
        Task<MarketplaceItemDto> CreateItemAsync(CreateMarketplaceItemDto dto);
        Task<MarketplaceItemDto> UpdateItemAsync(UpdateMarketplaceItemDto dto);
        Task<bool> DeleteItemAsync(int id, int userId);
        Task<bool> ToggleLikeAsync(int itemId, int userId);
        Task<IEnumerable<MarketplaceItemDto>> GetItemsByLocationAsync(CampusLocation location, int? currentUserId = null);
        Task<IEnumerable<MarketplaceItemDto>> GetItemsBySellerAsync(int sellerId, int? currentUserId = null);

    }
}
