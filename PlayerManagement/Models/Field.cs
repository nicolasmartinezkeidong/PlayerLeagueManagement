using System.ComponentModel.DataAnnotations;

namespace PlayerManagement.Models
{
    public class Field
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You must enter a park name.")]
        [StringLength(100, ErrorMessage = "Park name cannot be more than 100 characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "You must enter the address of the field.")]
        [StringLength(200, ErrorMessage = "Address cannot be more than 200 characters long.")]
        public string Address { get; set; }

        [StringLength(1000, ErrorMessage = "Comments cannot be more than 1000 characters long.")]
        [DataType(DataType.MultilineText)]
        public string? Comments { get; set; }

        [Required(ErrorMessage = "You must enter a Google Maps link.")]
        [StringLength(200, ErrorMessage = "Link cannot be more than 200 characters long.")]
        public string GoogleMapsLink { get; set; }
    }
}

