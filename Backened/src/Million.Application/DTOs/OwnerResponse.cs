using System;

namespace Million.Application.DTOs
{
    public class OwnerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public DateTime Birthday { get; set; }
        public string Username { get; set; }
    }
} 