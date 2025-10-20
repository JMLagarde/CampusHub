using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Application.DTO.Common
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string SessionToken { get; set; } = string.Empty; 
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? RedirectUrl { get; set; }
    }
}
