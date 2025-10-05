namespace CampusHub.Domain.Entities
{
    public class ImageBanner
    {
        public int Id { get; set; }
        public required string FileName { get; set; }           
        public required string ContentType { get; set; }       
        public required string Path { get; set; }              
        public long FileSize { get; set; }       
        public DateTime UploadedAt { get; set; }

    }
}
