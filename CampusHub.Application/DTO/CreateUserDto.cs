using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Application.DTO
{
    public class CreateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? StudentNumber { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public int? YearLevelId { get; set; }
        public int? ProgramID { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
