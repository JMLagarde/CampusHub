namespace CampusHub.Application.DTO
{
    public class ReportDto
    {
        public int Id { get; set; }
        public int MarketplaceItemId { get; set; }
        public string ItemTitle { get; set; } = string.Empty;
        public int ReporterId { get; set; }
        public string ReporterName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AdminNotes { get; set; }  
        public int? AdminUserId { get; set; }
    }
}
