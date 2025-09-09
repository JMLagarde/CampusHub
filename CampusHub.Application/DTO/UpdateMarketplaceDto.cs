using CampusHub.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CampusHub.Application.DTO
{
    public class UpdateMarketplaceDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between ₱0.01 and ₱999,999.99")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public ItemCategory Category { get; set; }

        [Required(ErrorMessage = "Condition is required")]
        public ItemCondition Condition { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public CampusLocation Location { get; set; }

        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Meetup preference is required")]
        public MeetupPreference MeetupPreference { get; set; }
    }
}
