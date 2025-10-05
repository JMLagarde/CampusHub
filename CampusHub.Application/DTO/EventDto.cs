namespace CampusHub.Application.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CollegeId { get; set; }
        public string CollegeName { get; set; } = string.Empty;
        public int? ProgramId { get; set; }
        public string? ProgramName { get; set; }
        public int CampusLocationId { get; set; }         
        public string CampusLocationName { get; set; } = string.Empty; 
        public DateTime Date { get; set; }
        public DateTime? RegistrationDeadline { get; set; }
        public string Location { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public string Type { get; set; } = string.Empty;

        public int InterestedCount { get; set; }
        public bool IsBookmarked { get; set; }

        public string? OrganizerName { get; set; }
    }
}