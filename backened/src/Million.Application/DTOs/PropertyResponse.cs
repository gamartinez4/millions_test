namespace Million.Application.DTOs
{
    public class PropertyResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public int Year { get; set; }
        public string OwnerId { get; set; }
        public bool ForSale { get; set; }
    }
} 