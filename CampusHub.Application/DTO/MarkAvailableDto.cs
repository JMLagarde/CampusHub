
using System.ComponentModel.DataAnnotations;

namespace CampusHub.Application.DTO
{
    public class MarkAvailableDto
    {
        [Required]
        public int ItemId { get; set; }

        [Required]
        public int SellerId { get; set; }
    }
}
