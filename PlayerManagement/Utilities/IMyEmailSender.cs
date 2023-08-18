using PlayerManagement.ViewModels;

namespace PlayerManagement.Utilities
{
    public interface IMyEmailSender
    {
        Task SendOneAsync(string name, string email, string subject, string htmlMessage);
        Task SendToManyAsync(EmailMessage emailMessage);
    }
}
