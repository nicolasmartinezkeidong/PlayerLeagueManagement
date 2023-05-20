namespace PlayerManagement.Models
{
    public class PlayPosition
    {
        public int PlayerId { get; set; }
        public Player Player { get; set; }

        public int PlayerPositionId { get; set; }
        public PlayerPosition PlayerPosition { get; set; }

        
    }
}
