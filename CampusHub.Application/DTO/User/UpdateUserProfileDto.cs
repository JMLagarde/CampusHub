using System.ComponentModel.DataAnnotations;

namespace CampusHub.Application.DTO.User
{
    public class UpdateUserProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string Program { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }
}
