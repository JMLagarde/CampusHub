using CampusHub.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CampusHub.Application.DTO
{
    public class UpdateMarketplaceDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ItemCategory Category { get; set; }
        public ItemCondition Condition { get; set; }
        public CampusLocation Location { get; set; }
        public string? ImageUrl { get; set; }
        public MeetupPreference MeetupPreference { get; set; }
    }
}
