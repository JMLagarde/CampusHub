using System.ComponentModel.DataAnnotations;

namespace CampusHub.Domain.Entities
{
    public class MarketplaceItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }
        public ItemCategory Category { get; set; }
        public ItemCondition Condition { get; set; }
        public CampusLocation Location { get; set; }
        public string? ImageUrl { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public int SellerId { get; set; }
        public string? ContactNumber { get; set; }

        public int LikesCount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public MeetupPreference MeetupPreference { get; set; }

        public MarketplaceItemStatus Status { get; set; } = MarketplaceItemStatus.Active;
        public virtual ICollection<MarketplaceLike> Likes { get; set; } = new List<MarketplaceLike>();
    }
}
