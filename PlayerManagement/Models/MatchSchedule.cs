using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace PlayerManagement.Models
{
    public class MatchSchedule
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter a date for the game.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "You must enter a time for the game.")]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        [Required(ErrorMessage = "You must enter a location for the game.")]
        public int LocationId { get; set; }
        public Location Location { get; set; }

        [Required(ErrorMessage = "You must enter the home team for the game.")]
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; }

        [Required(ErrorMessage = "You must enter the away team for the game.")]
        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; }

        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
    }
}
