using CampusHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CampusHub.Application.Interfaces;

namespace CampusHub.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<YearLevel> YearLevels { get; set; }
        public DbSet<ProgramEntity> Programs { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<MarketplaceItem> MarketplaceItems { get; set; }
        public DbSet<MarketplaceLike> MarketplaceLikes { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventBookmark> EventBookmarks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUserEntity(modelBuilder);
            ConfigureCollegeEntity(modelBuilder);
            ConfigureProgramEntity(modelBuilder);
            ConfigureYearLevelEntity(modelBuilder);
            ConfigureMarketplaceItemEntity(modelBuilder);
            ConfigureMarketplaceLikeEntity(modelBuilder);
            ConfigureReportEntity(modelBuilder);
            ConfigureEventEntity(modelBuilder);
            ConfigureEventBookmarkEntity(modelBuilder);

            DatabaseSeeder.SeedData(modelBuilder);
        }

        private void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.StudentNumber).HasMaxLength(8);
                entity.Property(e => e.Username).HasMaxLength(25).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(150);
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.Role).HasMaxLength(50);

                entity.HasOne(u => u.YearLevel)
                      .WithMany(y => y.Users)
                      .HasForeignKey(u => u.YearLevelId);

                entity.HasOne(u => u.Program)
                      .WithMany(p => p.Users)
                      .HasForeignKey(u => u.ProgramID);
            });
        }

        private void ConfigureCollegeEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<College>(entity =>
            {
                entity.HasKey(e => e.CollegeId);
                entity.Property(e => e.CollegeName).HasMaxLength(200).IsRequired();
            });
        }

        private void ConfigureProgramEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProgramEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(300).IsRequired();

                entity.HasOne(p => p.College)
                      .WithMany(c => c.Programs)
                      .HasForeignKey(p => p.CollegeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureYearLevelEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YearLevel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            });
        }

        private void ConfigureMarketplaceItemEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MarketplaceItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.SellerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.Condition).HasConversion<int>();
                entity.Property(e => e.Location).HasConversion<int>();

                entity.HasIndex(e => e.SellerId);
                entity.HasIndex(e => e.Location);
                entity.HasIndex(e => e.CreatedDate);
            });
        }

        private void ConfigureMarketplaceLikeEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MarketplaceLike>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.MarketplaceItemId, e.UserId }).IsUnique();

                entity.HasOne(e => e.MarketplaceItem)
                    .WithMany(e => e.Likes)
                    .HasForeignKey(e => e.MarketplaceItemId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureReportEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.AdminNotes).HasMaxLength(1000);

                entity.HasOne(r => r.MarketplaceItem)
                      .WithMany()
                      .HasForeignKey(r => r.MarketplaceItemId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Reporter)
                      .WithMany()
                      .HasForeignKey(r => r.ReporterUserID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.ResolvedByUser)
                      .WithMany()
                      .HasForeignKey(r => r.ResolvedByUserId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.MarketplaceItemId);
                entity.HasIndex(e => e.ReporterUserID);
                entity.HasIndex(e => e.CreatedAt);
            });
        }

        private void ConfigureEventEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.CampusLocation)
                      .HasConversion<int>()
                      .IsRequired();
                entity.Property(e => e.Status)
                      .HasConversion<int>()
                      .IsRequired();
                entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ImagePath).HasMaxLength(500);
                entity.Property(e => e.Priority).HasMaxLength(20);
                entity.Property(e => e.Type).HasMaxLength(50);
                entity.HasOne(e => e.College)
                      .WithMany()
                      .HasForeignKey(e => e.CollegeId)
                      .HasPrincipalKey(c => c.CollegeId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Program)
                      .WithMany()
                      .HasForeignKey(e => e.ProgramId)
                      .HasPrincipalKey(p => p.Id)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Organizer)
                      .WithMany()
                      .HasForeignKey(e => e.OrganizerId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(e => e.EventBookmarks)
                      .WithOne(eb => eb.Event)
                      .HasForeignKey(eb => eb.EventId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.CollegeId);
                entity.HasIndex(e => e.ProgramId);
                entity.HasIndex(e => e.CampusLocation);
                entity.HasIndex(e => e.StartDate);
                entity.HasIndex(e => e.CreatedAt);
            });
        }

        private void ConfigureEventBookmarkEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventBookmark>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.EventId, e.UserId }).IsUnique();

                entity.HasOne(e => e.Event)
                    .WithMany(e => e.EventBookmarks)
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.CreatedAt);
            });
        }
    }
}