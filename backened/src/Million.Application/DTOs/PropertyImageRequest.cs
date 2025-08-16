namespace Million.Application.DTOs
{
    public class PropertyImageRequest
    {
        public int PropertyId { get; set; }
        public required string FileUrl { get; set; }
        public bool Enabled { get; set; }
    }
} 