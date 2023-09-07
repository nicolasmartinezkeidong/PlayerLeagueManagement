using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace PlayerManagement.Models
{
    public class MatchSchedule
    {
        public int Id { get; set; }

        public string Summary
        {
            get
            {
                return $"{Date} {Time} {Field}";
            }
        }
        //Get 3 first letter of team's name
        public string HomeTeamAbbreviation
        {
            get
            {
                return $"{HomeTeam.Name.ToUpper().Substring(0, 3)}";
            }
        }


        //Get 3 first letter of team's name
        public string AwayTeamAbbreviation
        {
            get
            {
                return $"{AwayTeam.Name.ToUpper().Substring(0, 3)}";
            }
        }
        public int MatchDay { get; set; }//Track Match Day #

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "You must enter a date for the game.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "You must enter a time for the game.")]
        [RegularExpression("^(3:50|5:10|6:30|7:50)$", ErrorMessage = "Invalid time format, it must be any of this three times 3:50,5:10,6:30,7:50.")]
        public string Time { get; set; }

        [Display(Name ="Field")]
        [Required(ErrorMessage = "You must enter a location for the game.")]
        public int FieldId { get; set; }
        public Field Field { get; set; }

        [Display(Name = "Home Team")]
        [Required(ErrorMessage = "You must enter the home team for the game.")]
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; }
        [Display(Name = "Away Team")]
        [Required(ErrorMessage = "You must enter the away team for the game.")]
        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; }

        [Display(Name = "HT Score")]
        public int? HomeTeamScore { get; set; } = 0;

        [Display(Name = "AT Score")] 
        public int? AwayTeamScore { get; set; } = 0;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HomeTeam.Name == AwayTeam.Name)
            {
                yield return new ValidationResult("A team cannot play against itself.", new[] { "HomeTeamId", "AwayTeamId" });
            }
        }
    }
}
