namespace CampusHub.Application.DTO.User
{
    public class ProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public string Campus { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public UserStatsDto Statistics { get; set; } = new();
    }
}
