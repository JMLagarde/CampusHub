using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Domain.Entities
{
    public class College
    {
        public int CollegeId { get; set; }
        public string CollegeName { get; set; } = string.Empty;

        public List<ProgramEntity> Programs { get; set; } = new();
    }
}
