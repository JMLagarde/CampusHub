using CampusHub.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CampusHub.Application.DTO
{
    public class CurrentUserDto
    {

        public string Username { get; set; } = string.Empty;
        public int? YearLevelId { get; set; }
        public int? ProgramID { get; set; }

        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string StudentNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        public string Program { get; set; } = string.Empty;

        [Required]
        public string YearLevel { get; set; } = string.Empty;

        public CampusLocation? CampusLocation { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Profile completion percentage
        public int ProfileCompleteness { get; set; }

        // User role/type
        public string Role { get; set; } = "Student";

        // Avatar/Profile picture URL
        public string? ProfilePictureUrl { get; set; }
    }
}