using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusHub.Domain.Entities
{
    public class Report
    {
        public int Id { get; set; }

        public int MarketplaceItemId { get; set; }
        public int ReporterUserID { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public ReportStatus Status { get; set; } = ReportStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ADD THESE MISSING PROPERTIES:
        public DateTime? ResolvedAt { get; set; }
        public int? ResolvedByUserId { get; set; }
        public string? AdminNotes { get; set; }

        // ADD THESE NAVIGATION PROPERTIES:
        public virtual MarketplaceItem? MarketplaceItem { get; set; }
        public virtual User? Reporter { get; set; }
        public virtual User? ResolvedByUser { get; set; }
    }
}
