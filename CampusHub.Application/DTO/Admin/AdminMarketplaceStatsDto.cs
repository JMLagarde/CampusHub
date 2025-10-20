namespace CampusHub.Application.DTO.Admin
{
    public class AdminMarketplaceStatsDto
    {
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }
        public int SoldListings { get; set; }
        public int FlaggedListings { get; set; }
    }
}
