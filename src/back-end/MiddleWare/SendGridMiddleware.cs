using Microsoft.AspNetCore.DataProtection;
using System;
using System.Threading.Tasks;
using back_end.Interfaces.EmailVerifications;
using back_end.Interfaces;
using System.Data;
using Microsoft.Extensions.Logging;


public class EmailVerificationServices : IEmailVerificationService
{
    private readonly IDataProtector _protector;
    private readonly ISendGridEmailService _emailService;
    private readonly ILogger<EmailVerificationServices> _logger;


    public EmailVerificationServices(
       IDataProtectionProvider dataProtectionProvider,
       ISendGridEmailService emailService,
       ILogger<EmailVerificationServices> logger)
    {
        _protector = dataProtectionProvider.CreateProtector("EmailVerification.v1");
        _emailService = emailService;
        _logger = logger;
    }

    public string GenerateEmailVerificationToken(string userId, string email)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("User Id is Null or Email is Null");
        }
        var expirationTime = DateTime.UtcNow.AddHours(4);
        var data = $"{userId}|{email}|{expirationTime:0}";

        return _protector.Protect(data);
    }

    // Interface-compatible wrapper to satisfy IEmailVerificationService.GenerateVerificationToken
    public string GenerateVerificationToken(string userId, string email)
    {
        return GenerateEmailVerificationToken(userId, email);
    }

    public Task<(bool IsValid, string UserId, string Email)> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return Task.FromResult((false, (string)null, (string)null));
        }
        try
        {
            var unProtectData = _protector.Unprotect(token);
            var parts = unProtectData.Split("|");
            if (parts.Length != 3)
            {
                _logger.LogWarning("invalidToken");
                return Task.FromResult((false, (string)null, (string)null));
            }
            string UserId = parts[0];
            string Email = parts[1];
            DateTime expirationTime = DateTime.Parse(parts[2]);
            if (expirationTime > DateTime.UtcNow)
            {
                return Task.FromResult((true, UserId, Email));
            }
            return Task.FromResult((false, (string)null, (string)null));
        }
        catch
        {
            _logger.LogWarning("Issue Arose in processing a Email ValidateToke");
            return Task.FromResult((false, (string)null, (string)null));
        }
    }

    // Interface-compatible wrapper to satisfy IEmailVerificationService.ValidateTokeAsync
    public Task<(bool IsValid, string UserId, string Email)> ValidateTokeAsync(string token)
    {
        return ValidateTokenAsync(token);
    }
    
    public async Task SendVerificationEmailAsync(string email, string token)
    {
        try
        {
            await _emailService.SendEmailVerificationAsync(email, token);
            _logger.LogInformation("Verification email sent to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email to {Email}", email);
            throw;
        }
    }
    
    public async Task SendVerificationConfirmationAsync(string email)
    {
        try
        {
            await _emailService.SendEmailVerifiedConfirmationAsync(email);
            _logger.LogInformation("Verification confirmation sent to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to {Email}", email);
        }
    }    
}