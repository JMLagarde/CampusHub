
using CampusHub.Domain.Entities;

namespace CampusHub.Application.DTO
{
    public class MarketplaceItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ItemCondition Condition { get; set; }
        public DateTime Created { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public int SellerId { get; set; }
        public int LikesCount { get; set; }
        public bool IsLiked { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
        public ItemCategory Category { get; set; }
        public MeetupPreference MeetupPreference { get; set; }
    }
}
