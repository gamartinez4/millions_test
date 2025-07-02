using Million.Application.DTOs;

namespace Million.Application.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public OwnerResponse Owner { get; set; }
    }
} 