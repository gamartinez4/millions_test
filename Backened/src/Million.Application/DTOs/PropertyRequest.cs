namespace Million.Application.DTOs
{
    public class PropertyRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public int Year { get; set; }
        public int OwnerId { get; set; }
    }
} 