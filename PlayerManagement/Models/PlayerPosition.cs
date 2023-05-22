using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PlayerManagement.Models
{
    public class PlayerPosition
    {
        public int Id { get; set; }

        [Display(Name = "Player Position")]
        [Required(ErrorMessage = "You cannot leave the name of the player position blank.")]
        [StringLength(50, ErrorMessage = "Player position name cannot be more than 50 characters long.")]
        public string PlayerPos { get; set; }

        //[RegularExpression("^\\[A-Z]{2}$", ErrorMessage = "Code must be only letters.")]
        //[StringLength(3, ErrorMessage = "Code must be 2 or 3 letter characters.", MinimumLength = 2)]
        //public string PositionsAbbreviation { get; set; }

        [Display(Name = "Primary Player")]
        public ICollection<Player> Players { get; set; } = new HashSet<Player>();

        [Display(Name = "Other Players")]
        public ICollection<PlayPosition> Plays { get; set; } = new HashSet<PlayPosition>();

        
    }
}
