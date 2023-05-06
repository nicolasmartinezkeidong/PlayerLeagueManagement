
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PlayerManagement.Models
{
    public class League
    {
        public int Id { get; set; }

        [Display(Name = "League")]
        public string FullSummary
        {
            get
            {
                return Name + " - " + LeagueFoundation;
            }
        }

        [Required(ErrorMessage = "You cannot leave the League name blank.")]
        [StringLength(50, ErrorMessage = "League name cannot be more than 50 characters long.")]
        public string Name { get; set; }

        [Display(Name = "League Foundation")]
        [Required(ErrorMessage = "You cannot leave the League Foundation blank.")]
        [RegularExpression("^\\d{4}$", ErrorMessage = "The League Foundation must be entered as exactly 4 numeric digits.")]
        [StringLength(4)]
        public string LeagueFoundation { get; set; }

        [Required(ErrorMessage = "You must enter a budget for the League.")]
        [Range(5000.00, 200000.00, ErrorMessage = "Budget must be between $5000.00 and $200,000.00")]
        [DataType(DataType.Currency)]
        public double LeagueBudget { get; set; } = 1000.00d; //Default: Avoid validation error if you create
                                                             //the object without supplying a value.
                                                             //[Display(Name = "Inventory")]
                                                             //public int InStock { get; set; } = 0;
        [Display(Name ="# Teams")]
        public int NumberOfTeams { get; set; }

        public ICollection<Team> Teams { get; set; } = new HashSet<Team>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((int.Parse(LeagueFoundation) - 1) > DateTime.Today.Year)
            {
                yield return new ValidationResult("League Foundation cannot be more then one year in the future.", new[] { "LeagueFoundation" });
            }
        }
    }
}
