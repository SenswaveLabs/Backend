using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using Senswave.Users.Domain.Interfaces;
using Senswave.Users.Infrastructure.Options;

namespace Senswave.Users.Infrastructure.Services;

internal class MailkitEmailService(
    IOptionsMonitor<EmailServiceOptions> options,
    ILogger<MailkitEmailService> logger) : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        if (!options.CurrentValue.Enabled)
        {
            logger.LogWarning("Email service is disabled. Email not sent.");
            return;
        }

        logger.LogInformation("Sending email.");
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));

        try
        {
            var message = new MimeMessage
            {
                Subject = subject,
                Body = new TextPart(TextFormat.Html) { Text = body }
            };
            message.To.Add(MailboxAddress.Parse(to));

            if (string.IsNullOrEmpty(options.CurrentValue.SenderName))
            {
                message.From.Add(MailboxAddress.Parse(options.CurrentValue.SenderEmail));
            }
            else
            {
                message.From.Add(new MailboxAddress(
                    options.CurrentValue.SenderName,
                    options.CurrentValue.SenderEmail
                ));
            }

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(options.CurrentValue.Host, options.CurrentValue.Port, options.CurrentValue.UseSsl, cts.Token);
            await smtp.AuthenticateAsync(options.CurrentValue.User, options.CurrentValue.Password, cts.Token);
            await smtp.SendAsync(message, cts.Token);
            await smtp.DisconnectAsync(true, cts.Token);

            logger.LogInformation("Email sent successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending email.");
        }
    }
}
