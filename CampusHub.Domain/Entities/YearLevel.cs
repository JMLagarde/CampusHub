using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Domain.Entities
{
    public class YearLevel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation
        public List<User> Users { get; set; } = new();
    }
}
