using CampusHub.Domain.Entities;

namespace CampusHub.Application.DTO.Events
{
    public class EventListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CollegeName { get; set; } = string.Empty;
        public string? ProgramName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public int InterestedCount { get; set; }
        public bool IsBookmarked { get; set; }
    }
}
