namespace Million.Application.DTOs
{
    public class PropertyImageResponse
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string FileUrl { get; set; }
        public bool Enabled { get; set; }
    }
} 