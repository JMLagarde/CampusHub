using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Domain.Entities
{
    public class User
    {
        public int UserID { get; set; }
        public string? StudentNumber { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }
        public string? ContactNumber { get; set; }

        public string? Role { get; set; } = "Student";
        public DateTime? DateRegistered { get; set; } = DateTime.UtcNow; 

        [Required]
        public string Password { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }

        public string? ProfilePictureUrl { get; set; }

        // Foreign Keys
        public int? YearLevelId { get; set; }
        public int? ProgramID { get; set; }

        // Navigation Properties
        public YearLevel? YearLevel { get; set; }
        public ProgramEntity? Program { get; set; }
        public ICollection<MarketplaceItem> MarketplaceItems { get; set; } = new List<MarketplaceItem>();
        public ICollection<MarketplaceLike> MarketplaceLikes { get; set; } = new List<MarketplaceLike>();
        public ICollection<MeetupPreference> MeetupPreferences { get; set; } = new List<MeetupPreference>();


        // Domain Logic
        public bool IsStudent() => Role?.Equals("Student", StringComparison.OrdinalIgnoreCase) ?? false;
        public bool IsAdmin() => Role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) ?? false;
        public string GetDisplayName() => !string.IsNullOrEmpty(FullName) ? FullName : Username;
    }
}