namespace PlayerManagement.Models
{
    public class Play
    {
        public int PlayerPositionId { get; set; }
        public PlayerPosition PlayerPosition { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }
    }
}
