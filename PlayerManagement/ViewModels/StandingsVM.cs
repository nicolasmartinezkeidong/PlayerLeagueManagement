using PlayerManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace PlayerManagement.ViewModels
{
    public class StandingsVM
    {
        public int Id { get; set; }
        public int Position { get; set; }

        [Display(Name ="Team")]
        public string TeamName { get; set; }
        
        public int Played { get; set; }
        public int Won { get; set; }
        public int Drawn { get; set; }
        public int Lost { get; set; }

        [Display(Name ="GF")]
        public int GoalsFavor { get; set; }

        [Display(Name = "GA")]
        public int GoalsAgainst { get; set; }

        [Display(Name = "GD")]
        public int GoalsDifference { get; set; }
        public int Points { get; set; }
        public string Form { get; set; }
    }
}
