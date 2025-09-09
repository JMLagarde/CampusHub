using System.ComponentModel.DataAnnotations;

namespace CampusHub.Application.DTO
{
    public class UpdateUserProfileDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string StudentNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(15)]
        public string? ContactNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Program { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string YearLevel { get; set; } = string.Empty;

        public string? ProfilePictureUrl { get; set; }
    }
}
