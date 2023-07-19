using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PlayerManagement.ViewModels
{
    public class TeamStatsVM
    {
        public int Id { get; set; }

        [Display(Name = "Team")]
        public string TeamName { get; set; }

        public int Goals { get; set; }

        public int RedCards { get; set; }

        public int YellowCards { get; set; }
    }
}
