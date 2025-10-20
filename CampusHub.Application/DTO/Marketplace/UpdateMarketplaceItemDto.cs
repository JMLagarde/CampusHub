using CampusHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Application.DTO.Marketplace
{
    public class UpdateMarketplaceItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ItemCategory Category { get; set; }
        public MeetupPreference MeetupPreference { get; set; }
        public decimal Price { get; set; }
        public ItemCondition Condition { get; set; }
        public CampusLocation Location { get; set; }
        public string? ImageUrl { get; set; }
        public string? ContactNumber { get; set; }

    }
}

