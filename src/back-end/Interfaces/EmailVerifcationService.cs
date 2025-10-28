
namespace back_end.Interfaces.EmailVerifications
{
    public interface IEmailVerificationService
    {
        string GenerateVerificationToken(string userId, string email);
        Task<(bool IsValid, string UserId, string Email)> ValidateTokeAsync(string token);
        Task SendVerificationEmailAsync(string email, string tokem);
        Task SendVerificationConfirmationAsync(string email);
    }

    public interface ISendGridEmailService
    {
        Task SendEmailVerificationAsync(string toEmail, string token);
        Task SendEmailVerifiedConfirmationAsync(string toEmail);
    }
}