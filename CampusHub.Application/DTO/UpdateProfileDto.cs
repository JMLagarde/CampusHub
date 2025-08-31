using System.ComponentModel.DataAnnotations;

namespace CampusHub.Application.DTO
{
    public class UpdateProfileDto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string ContactNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string ProgramID { get; set; } = string.Empty;

        [StringLength(20)]
        public string YearLevelID { get; set; } = string.Empty;

        [StringLength(50)]
        public string CampusLocation { get; set; } = string.Empty;
    }
}
