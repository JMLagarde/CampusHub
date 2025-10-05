using System;
using CampusHub.Domain.Entities;

namespace CampusHub.Domain.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int CollegeId { get; set; }
        public College? College { get; set; }
        public int? ProgramId { get; set; }
        public ProgramEntity? Program { get; set; }
        public CampusLocation CampusLocation { get; set; } = CampusLocation.MainCampus; 
        public DateTime Date { get; set; }
        public DateTime? RegistrationDeadline { get; set; }
        public string Location { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public string Type { get; set; } = string.Empty;
        public int InterestedCount { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int? OrganizerId { get; set; }
        public User? Organizer { get; set; }
    }
}