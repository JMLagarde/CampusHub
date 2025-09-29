using CampusHub.Domain.Entities;
using System.ComponentModel.DataAnnotations;


namespace CampusHub.Application.DTO
{
    public class UpdateReportStatusDto
    {
        [Required]
        public ReportStatus Status { get; set; }

        [Required]
        public int AdminUserId { get; set; }

        [StringLength(500)]
        public string? AdminNotes { get; set; }
    }
}
