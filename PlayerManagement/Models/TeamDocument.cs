using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PlayerManagement.Models
{
    public class TeamDocument : UploadedFile
    {
        [Display(Name = "Team")]
        public int TeamId { get; set; }

        public Team Team { get; set; }
    }
}
