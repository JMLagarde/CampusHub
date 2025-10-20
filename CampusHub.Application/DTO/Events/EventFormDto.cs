using CampusHub.Domain.Entities;

namespace CampusHub.Application.DTO.Events
{
    public class EventFormDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CollegeId { get; set; }
        public int? ProgramId { get; set; }
        public int CampusLocationId { get; set; } = 1;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddHours(1);
        public DateTime? RegistrationDeadline { get; set; }
        public string Location { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public string Type { get; set; } = string.Empty;
        public int InterestedCount { get; set; }
    }
}
