using SWD.BLL.Interfaces;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;

namespace SWD.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _apiKey = _configuration["SENDGRID_API_KEY"] 
                ?? Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ?? "";

            _fromEmail = _configuration["EMAIL_FROM"] 
                ?? Environment.GetEnvironmentVariable("EMAIL_FROM") ?? "";

            _fromName = _configuration["EMAIL_FROM_NAME"] 
                ?? Environment.GetEnvironmentVariable("EMAIL_FROM_NAME")
                ?? "Smart Weather Data Lab";

            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_fromEmail))
            {
                _logger.LogWarning("EmailService initialized containing MISSING CONFIGURATION (SENDGRID_API_KEY or EMAIL_FROM). Email sending will fail.");
            }
            else
            {
                _logger.LogInformation($"EmailService initialized with SendGrid - From: {_fromEmail}, Name: {_fromName}, API Key: ***SET***");
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_fromEmail))
            {
                _logger.LogError("Cannot send email: Update SendGrid configuration (SENDGRID_API_KEY and EMAIL_FROM) in environment variables.");
                // We can throw or just return. Since it returns Task, throwing propagates to caller.
                // The caller (UserController) ignores the Task (fire-and-forget), but logs exceptions.
                // It is better to throw so the logs show the error clearly.
                throw new InvalidOperationException("EmailService is not configured properly.");
            }

            _logger.LogInformation($"Preparing to send email via SendGrid to {toEmail} with subject: {subject}");

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, _fromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);

            _logger.LogInformation($"Sending email via SendGrid API to {toEmail}");
            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Email sent successfully via SendGrid to {toEmail}. Status: {response.StatusCode}");
            }
            else
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                _logger.LogError($"SendGrid API failed. Status: {response.StatusCode}, Error: {errorBody}");
                throw new Exception($"SendGrid failed with status {response.StatusCode}: {errorBody}");
            }
        }
    }
}
