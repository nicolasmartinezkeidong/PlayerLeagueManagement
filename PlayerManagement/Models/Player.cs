﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Xml.Linq;

namespace PlayerManagement.Models
{
    public class Player : Auditable
    {
        public int Id { get; set; }

        [Display(Name = "Player")]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public int Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int a = today.Year - DOB.Year
                    - ((today.Month < DOB.Month || (today.Month == DOB.Month && today.Day < DOB.Day) ? 1 : 0));
                return a; /*Note: You could add .PadLeft(3) but spaces disappear in a web page. */
            }
        }

        [Display(Name = "Phone")]
        public string PhoneFormatted
        {
            get
            {
                return "(" + Phone.Substring(0, 3) + ") " + Phone.Substring(3, 3) + "-" + Phone[6..];
            }
        }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(30, ErrorMessage = "First name cannot be more than 30 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(50, ErrorMessage = "Last name cannot be more than 50 characters long.")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "You must enter the Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DOB { get; set; }

        [Display(Name = "Games Played")]
        [Range(0, int.MaxValue, ErrorMessage = "Games Played must be a positive number.")]
        public int GamesPlayed { get; set; } = 0;

        [Display(Name = "Team")]
        [Required(ErrorMessage = "You must select a Team.")]
        public int TeamId { get; set; }
        public Team Team { get; set; }

        [Display(Name = "Primary Position")]
        [Required(ErrorMessage = "You must select the principal position the player plays.")]
        public int PlayerPositionId { get; set; }
        [Display(Name = "Primary Position")]
        public PlayerPosition PlayerPosition { get; set; }

        [Display(Name = "All Positions")]
        public ICollection<PlayPosition> Plays { get; set; } = new HashSet<PlayPosition>();

        [Display(Name = "Documents")]
        public ICollection<PlayerDocument> PlayerDocuments { get; set; } = new HashSet<PlayerDocument>();

        public PlayerPhoto PlayerPhoto { get; set; }

        public PlayerThumbnail PlayerThumbnail { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DOB > DateTime.Today)
            {
                yield return new ValidationResult("Date of Birth cannot be in the future.", new[] { "DOB" });
            }
            else if (Age < 16)
            {
                yield return new ValidationResult("Player must be at least 16 years old.", new[] { "DOB" });
            }
        }
    }
}