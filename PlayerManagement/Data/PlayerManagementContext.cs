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
        public DbSet<Field> Fields { get; set; }
        public DbSet<MatchSchedule> MatchSchedules { get; set; } 

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
            modelBuilder.Entity<Team>()
                .HasMany<Player>(t => t.Players)
                .WithOne(t => t.Team)
                .HasForeignKey(t => t.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            //Add a unique index to the Player Email and Phone
            modelBuilder.Entity<Player>()
            .HasIndex(p => new { p.Email, p.Phone })
            .IsUnique();

            //Many to Many Primary Key
            modelBuilder.Entity<Play>()
            .HasKey(p => new { p.PlayerId, p.PlayerPositionId });

            //Add a unique index to the Team Name
            modelBuilder.Entity<Team>()
            .HasIndex(p => new { p.Name })
            .IsUnique();
            //Add a unique index to the League Name
            modelBuilder.Entity<League>()
            .HasIndex(p => new { p.Name })
            .IsUnique();
        }
    }
}
