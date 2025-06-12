using System;

namespace Million.Domain.Models
{
    public class Owner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public DateTime Birthday { get; set; }

        public Owner(string name, string address, string photo, DateTime birthday)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address cannot be empty.", nameof(address));
            }

            Name = name;
            Address = address;
            Photo = photo;
            Birthday = birthday;
        }
    }
} 