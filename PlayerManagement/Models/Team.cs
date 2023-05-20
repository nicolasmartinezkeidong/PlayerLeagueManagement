
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PlayerManagement.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You cannot leave the Team name blank.")]
        [StringLength(80, ErrorMessage = "Song title cannot be more than 80 characters long.")]
        public string Name { get; set; }

        [Display(Name = "Registration Date ")]
        [Required(ErrorMessage = "You must enter the Registration Date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDate { get; set; }

        [Display(Name = "League")]
        [Required(ErrorMessage = "You must select one League.")]
        public int LeagueId { get; set; }
        public League League { get; set; }

        [Display(Name = "Players")]
        public ICollection<Player> Players { get; set; } = new HashSet<Player>();

        [Display(Name = "Others Players")]
        public ICollection<PlayerTeam> PlayersTeams { get; set; } = new HashSet<PlayerTeam>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (Name[0] == '@' || Name[0] == '$' || Name[0] == '&')
            {
                yield return new ValidationResult("Team names are not allowed to start with the letters @, $, or &.", new[] { "Name" });
            }

        }
    }
}
