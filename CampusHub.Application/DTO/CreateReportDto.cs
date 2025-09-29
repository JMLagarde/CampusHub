using System.ComponentModel.DataAnnotations;

namespace CampusHub.Application.DTO
{
    public class CreateReportDto
    {
        public int MarketplaceItemId { get; set; }
        public int ReporterId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}