using System.ComponentModel.DataAnnotations;

namespace PlayerManagement.Models
{
    public class NewsPhoto
    {
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public byte[] Content { get; set; }

        [StringLength(255)]
        public string MimeType { get; set; }

        public int NewsId { get; set; }
        public News News { get; set; }
    }
}