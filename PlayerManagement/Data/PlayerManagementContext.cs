using Microsoft.EntityFrameworkCore;
using PlayerManagement.Models;
using System.Numerics;

namespace PlayerManagement.Data
{
    public class PlayerManagementContext : DbContext
    {
        public PlayerManagementContext(DbContextOptions<PlayerManagementContext> options)
                    : base(options)
        {
        }

        public DbSet<League> Leagues { get; set; }
        public DbSet<Play> Plays { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerPosition> PlayerPositions { get; set; }
        public DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("SL");

            //Prevent Cascade Delete from League to Team
            modelBuilder.Entity<League>()
                .HasMany<Team>(t => t.Teams)
                .WithOne(t => t.League)
                .HasForeignKey(t => t.LeagueId)
                .OnDelete(DeleteBehavior.Restrict);

            //Add a unique index to the 
            modelBuilder.Entity<Player>()
            .HasIndex(p => new { p.Email, p.Phone })
            .IsUnique();

            //Many to Many Primary Key
            modelBuilder.Entity<Play>()
            .HasKey(p => new { p.PlayerId, p.PlayerPositionId });

            //Add a unique index
            modelBuilder.Entity<Team>()
            .HasIndex(p => new { p.Name })
            .IsUnique();
        }
    }
}
