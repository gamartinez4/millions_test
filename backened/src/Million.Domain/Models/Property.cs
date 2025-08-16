using System.Collections.Generic;

namespace Million.Domain.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public int Year { get; set; }
        public int? OwnerId { get; set; }
        public bool ForSale { get; set; }

        public Property(string name, string address, decimal price, int year, int? ownerId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address cannot be empty.", nameof(address));
            }
            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
            }

            Name = name;
            Address = address;
            Price = price;
            Year = year;
            OwnerId = ownerId;
            ForSale = ownerId == null;

        }

        public Property(string name, string address, decimal price, int year, int? ownerId, bool forSale)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address cannot be empty.", nameof(address));
            }
            if (price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
            }

            Name = name;
            Address = address;
            Price = price;
            Year = year;
            OwnerId = ownerId;
            ForSale = forSale;

        }
    }
} 