using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Xml.Linq;

namespace PlayerManagement.Models
{
    public class PlayerDocument : UploadedFile
    {
        [Display(Name = "Player")]
        public int PlayerId { get; set; }

        public Player Player { get; set; }
    }
}
