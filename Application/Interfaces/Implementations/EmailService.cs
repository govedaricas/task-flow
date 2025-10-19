using Application.Exceptions;
using Application.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.Interfaces.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IConfiguration config, IOptions<SmtpSettings> smtpSettings)
        {
            _configuration = config;
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message, CancellationToken cancellationToken)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            try
            {
                using var client = new SmtpClient();

                await client.ConnectAsync(_smtpSettings.Server, int.Parse(_smtpSettings.Port), MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password, cancellationToken);
                await client.SendAsync(emailMessage, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (SmtpCommandException ex)
            {
                throw new ConflictException("Email", $"SMTP command failed: {ex.Message}");
            }
            catch (SmtpProtocolException ex)
            {
                throw new ConflictException("Email", $"SMTP protocol error occurred while sending email. {ex.Message}");
            }
            catch (AuthenticationException ex)
            {
                throw new ConflictException("Email", $"SMTP authentication failed. {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new ConflictException("Email", $"Failed to send email. See logs for details. {ex.Message}");
            }
        }
    }
}
