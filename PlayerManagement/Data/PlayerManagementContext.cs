using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PlayerManagement.Models;
using System.Numerics;
using PlayerManagement.ViewModels;

namespace PlayerManagement.Data
{
    public class PlayerManagementContext : DbContext
    {
        //To give access to IHttpContextAccessor for Audit Data with IAuditable
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the UserName value
        public string UserName
        {
            get; private set;
        }

        public PlayerManagementContext(DbContextOptions<PlayerManagementContext> options, IHttpContextAccessor httpContextAccessor)
                    : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            if (_httpContextAccessor.HttpContext != null)
            {
                //We have a HttpContext, but there might not be anyone Authenticated
                UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
                UserName ??= "Unknown";
            }
            else
            {
                //No HttpContext so seeding data. If else is true that means that is seed data
                UserName = "Seed Data";
            }
        }

        public DbSet<League> Leagues { get; set; }
        public DbSet<PlayPosition> Plays { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerPosition> PlayerPositions { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<MatchSchedule> MatchSchedules { get; set; }
        public DbSet<PlayerDocument> PlayerDocuments { get; set; }
        public DbSet<TeamDocument> TeamDocuments { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<PlayerPhoto> PlayerPhotos { get; set; }
        public DbSet<PlayerThumbnail> PlayerThumbnails { get; set; }
        public DbSet<PlayerMatch> PlayerMatchs { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsPhoto> NewsPhotos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("SL");

            //Prevent Cascade Delete from League to Team
            modelBuilder.Entity<League>()
                .HasMany<Team>(t => t.Teams)
                .WithOne(t => t.League)
                .HasForeignKey(t => t.LeagueId)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Team to Player
            //modelBuilder.Entity<Team>()
            //    .HasMany<Player>(t => t.Players)
            //    .WithOne(t => t.Team)
            //    .HasForeignKey(t => t.TeamId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //Add a unique index to the Player Email and Phone
            modelBuilder.Entity<Player>()
            .HasIndex(p => new { p.Email, p.Phone })
            .IsUnique();

            //Many to Many Primary Key
            modelBuilder.Entity<PlayPosition>()
            .HasKey(p => new { p.PlayerId, p.PlayerPositionId });

            //Many to Many Primary Key
            modelBuilder.Entity<PlayerTeam>()
            .HasKey(p => new { p.PlayerId, p.TeamId });

            //Add a unique index to the Team Name
            modelBuilder.Entity<Team>()
            .HasIndex(p => new { p.Name })
            .IsUnique();
            //Add a unique index to the League Name
            modelBuilder.Entity<League>()
            .HasIndex(p => new { p.Name })
            .IsUnique();

            //Add a unique index to the Position Name
            modelBuilder.Entity<PlayerPosition>()
            .HasIndex(p => new { p.PlayerPos })
            .IsUnique();
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }

        public DbSet<PlayerManagement.ViewModels.TeamStatsVM> TeamStatsVM { get; set; }
        public DbSet<PlayerManagement.ViewModels.StandingsVM> StandingsVM { get; set; }
    }
}
