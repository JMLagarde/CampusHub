using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using CampusHub.Application.DTO;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<YearLevel> YearLevels { get; set; }
        DbSet<ProgramEntity> Programs { get; set; }
        DbSet<College> Colleges { get; set; }
        DbSet<MarketplaceItem> MarketplaceItems { get; set; }
        DbSet<MarketplaceLike> MarketplaceLikes { get; set; }
        DbSet<Report> Reports { get; set; }
        DbSet<Event> Events { get; set; }
        DbSet<EventBookmark> EventBookmarks { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}
