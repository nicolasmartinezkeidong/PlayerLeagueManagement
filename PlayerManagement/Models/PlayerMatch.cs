using System.ComponentModel.DataAnnotations;

namespace PlayerManagement.Models
{
    public class PlayerMatch
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "You must enter the number of goals scored.")]
        [Range(0, int.MaxValue, ErrorMessage = "Goals must be a positive number.")] 
        public int Goals { get; set; } = 0;

        [Required(ErrorMessage = "You cannot leave the notes blank.")]
        [StringLength(2000, ErrorMessage = "Only 2000 characters for notes.")]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Display(Name = "Red Cards")]
        //[Required(ErrorMessage = "You must enter the number of red cards received.")]
        [Range(0, int.MaxValue, ErrorMessage = "Red cards must be a positive number.")]
        public int? RedCards { get; set; } = 0;

        [Display(Name ="Yellow Cards")]
        //[Required(ErrorMessage = "You must enter the number of yellow cards received.")]
        [Range(0, int.MaxValue, ErrorMessage = "Yellow cards must be a positive number.")]
        public int? YellowCards { get; set; } = 0;

        [Display(Name = "Matches Played")]
        [Required(ErrorMessage = "You must select a match.")]
        public int? MatchId { get; set; }
        public MatchSchedule Match { get; set; }

        [Required(ErrorMessage = "You must select a player.")]
        public int PlayerId { get; set; }
        public virtual Player Player { get; set; }
    }
}
