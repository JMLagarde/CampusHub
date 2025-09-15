using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Application.DTO
{
    public class MarkItemStatusDto
    {
        [Required]
        public int ItemId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
