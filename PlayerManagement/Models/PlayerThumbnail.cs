using System.ComponentModel.DataAnnotations;

namespace PlayerManagement.Models
{
    public class PlayerThumbnail
    {
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public byte[] Content { get; set; }

        [StringLength(255)]
        public string MimeType { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }
    }
}
