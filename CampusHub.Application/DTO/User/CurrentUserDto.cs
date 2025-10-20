using CampusHub.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CampusHub.Application.DTO.User
{
    public class CurrentUserDto
    {
        public string Username { get; set; } = string.Empty;
        public int? YearLevelId { get; set; }
        public int? ProgramID { get; set; }
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public CampusLocation? CampusLocation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public int ProfileCompleteness { get; set; }
        public string Role { get; set; } = "Student";
        public string? ProfilePictureUrl { get; set; }
    }
}