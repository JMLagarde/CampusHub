namespace CampusHub.Application.DTO.Admin
{
    public class AdminUserDto
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? StudentNumber { get; set; }
        public string? Program { get; set; }
        public string? Year { get; set; }
        public string? Role { get; set; }
        public string Status { get; set; } = "Active";
        public int ReportsCount { get; set; }
        public int ListingsCount { get; set; }
        public string? JoinDate { get; set; }
        public bool Verified { get; set; }
        public DateTime? DateRegistered { get; set; }
    }
}
