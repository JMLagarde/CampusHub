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
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string Username { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Student Number cannot exceed 20 characters")]
        public string? StudentNumber { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Please enter a valid contact number")]
        [StringLength(20, ErrorMessage = "Contact Number cannot exceed 20 characters")]
        public string? ContactNumber { get; set; }

        public int? YearLevelId { get; set; }

        public int? ProgramID { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = string.Empty;
    }
}
