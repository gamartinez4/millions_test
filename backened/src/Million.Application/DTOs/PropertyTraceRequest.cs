using System;

namespace Million.Application.DTOs
{
    public class PropertyTraceRequest
    {
        public int Id { get; set; }
        public DateTime DateSale { get; set; }
        public required string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Tax { get; set; }
        public int PropertyId { get; set; }
    }
} 