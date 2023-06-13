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

        public ICollection<MatchSchedule> Matches { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Matches.Any(m => m.FieldId == FieldId && m.Time == Time))
            {
                yield return new ValidationResult("There is already a match scheduled for field.", new[] { "FieldId", "Time" });
            }

            else if (HomeTeam.Name == AwayTeam.Name)
            {
                yield return new ValidationResult("A team cannot play against itself.", new[] { "HomeTeamId", "AwayTeamId" });
            }

            else if (Matches.Any(m => m.Date == Date && (m.HomeTeamId == HomeTeamId || m.AwayTeamId == AwayTeamId)))
            {
                yield return new ValidationResult("One of the teams has already have a match scheduled.", new[] { "Date", "HomeTeamId", "AwayTeamId" });
            }
        }
    }
}
