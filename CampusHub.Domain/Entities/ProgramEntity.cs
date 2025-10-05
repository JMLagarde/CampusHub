using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Domain.Entities
{
    public class ProgramEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CollegeId { get; set; }

        // Navigation
        public College? College { get; set; }
        public List<User> Users { get; set; } = new();
        public List<Event> Events { get; set; } = new();
    }
}

