using Microsoft.AspNetCore.Http;

namespace API.Entities // Adjust the namespace if necessary
{
    public class AppUserUpdateDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Country { get; set; }
        public string? Gender { get; set; }
        public IFormFile? ProfilePhoto { get; set; } // For profile photo upload
    }
}
