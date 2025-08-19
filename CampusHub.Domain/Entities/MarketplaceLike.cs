using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusHub.Domain.Entities
{
    public class MarketplaceLike
    {
        public int Id { get; set; }
        public int MarketplaceItemId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public virtual MarketplaceItem MarketplaceItem { get; set; } = null!;
    }
}
