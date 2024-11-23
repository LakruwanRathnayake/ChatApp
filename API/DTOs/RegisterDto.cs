using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class RegisterDto
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; } // Add Email field
        public string? Phone { get; set; }

        public string? Country { get; set; }
        public string? Gender { get; set; }
    }
}