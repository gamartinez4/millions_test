using System;

namespace Million.Application.DTOs
{
    public class OwnerRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Photo { get; set; }
        public DateTime Birthday { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
} 