using System.ComponentModel.DataAnnotations;

namespace PlayerManagement.Models
{
    public class News
    {
        public int Id { get; set; }

        [Display(Name = "Author")]
        public string FullName
        {
            get
            {
                return $"{AuthorFirstName} {AuthorLastName}";
            }
        }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "You cannot leave the title blank.")]
        [StringLength(300, ErrorMessage = "Title cannot be more than 300 characters long.")]
        public string Title { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(30, ErrorMessage = "First name cannot be more than 30 characters long.")]
        public string AuthorFirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(50, ErrorMessage = "Last name cannot be more than 50 characters long.")]
        public string AuthorLastName { get; set; }

        [Required]
        [Display(Name = "Publication Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "News Image")]
        public NewsPhoto NewsPhoto { get; set; }

        [Display(Name = "Content")]
        [Required(ErrorMessage = "You cannot leave the content blank.")]
        [StringLength(700, ErrorMessage = "Title cannot be more than 700 characters long.")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
    }
}