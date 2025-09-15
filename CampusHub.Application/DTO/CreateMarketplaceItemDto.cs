using CampusHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Application.DTO
{
    public class CreateMarketplaceItemDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(1, 999999, ErrorMessage = "Price must be between ₱1 and ₱999,999")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Condition is required")]
        public ItemCondition Condition { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public CampusLocation Location { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ImageUrl { get; set; }

        public int SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public ItemCategory Category { get; set; }
        public MeetupPreference MeetupPreference { get; set; }
        [Phone]
        [StringLength(20)]
        public string? ContactNumber { get; set; }

    }
}
