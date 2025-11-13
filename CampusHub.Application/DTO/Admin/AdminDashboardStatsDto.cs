namespace CampusHub.Application.DTO.Admin
{
    public class AdminDashboardStatsDto
    {
        public int TotalStudents { get; set; }
        public int ActiveListings { get; set; }
        public int PublishedEvents { get; set; }
        public int PendingReports { get; set; }
        public List<CategoryDistributionDto> CategoryDistribution { get; set; } = new();
        public List<CollegeDistributionDto> CollegeDistribution { get; set; } = new();
    }

    public class CategoryDistributionDto
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class CollegeDistributionDto
    {
        public string College { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
