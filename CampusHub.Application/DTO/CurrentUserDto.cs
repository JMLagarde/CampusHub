namespace CampusHub.Application.DTO
{
    public class CurrentUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? StudentNumber { get; set; }
        public string? ContactNumber { get; set; }
        public int? YearLevelId { get; set; }
        public int? ProgramID { get; set; }
    }
}