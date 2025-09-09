using System.ComponentModel.DataAnnotations;

namespace CampusHub.Application.DTO
{
    public class ToggleLikeDto
    {
        public int ItemId { get; set; }
        public int MarketplaceItemId { get; set; }
        public int UserId { get; set; }
    }
}
