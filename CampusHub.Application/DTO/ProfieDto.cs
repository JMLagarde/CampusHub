namespace CampusHub.Application.DTO
{
    public class ProfieDto
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
        public ProfileStatisticsDto Statistics { get; set; } = new();
    }
}
