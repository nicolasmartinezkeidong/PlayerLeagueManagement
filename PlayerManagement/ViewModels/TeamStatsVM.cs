using System.ComponentModel.DataAnnotations;

namespace PlayerManagement.ViewModels
{
    public class TeamStatsVM
    {
        public int ID { get; set; }

        [Display(Name = "Team")]
        public string TeamName { get; set; }

        public int Goals { get; set; }

        public int RedCards { get; set; }

        public int YellowCards { get; set; }
    }
}
