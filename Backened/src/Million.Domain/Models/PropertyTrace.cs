using System;

namespace Million.Domain.Models
{
    public class PropertyTrace
    {
        public int Id { get; set; }
        public DateTime DateSale { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Tax { get; set; }
        public int PropertyId { get; set; }
     

        public PropertyTrace(int id,DateTime dateSale, string name, decimal value, decimal tax, int propertyId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            }
            if (value <= 0)
            {
                throw new ArgumentException("Value must be greater than zero.", nameof(value));
            }

            DateSale = dateSale;
            Name = name;
            Value = value;
            Tax = tax;
            PropertyId = propertyId;
        }
    }
} 