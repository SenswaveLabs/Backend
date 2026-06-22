using Microsoft.AspNetCore.Identity;
using Senswave.Users.Domain.Entity;
using Senswave.Users.Domain.Interfaces;
using Senswave.Users.Infrastructure.Options;

namespace Senswave.Users.Infrastructure.Services;

internal class HtmlEmailSender(
    IOptionsMonitor<EmailSenderOptions> emailOptions,
    IEmailService emailService,
    ILogger<HtmlEmailSender> logger) : IEmailSender<User>, IDeleteAccountService
{
    const string confirmationSubject = "Confirm your email";

    const string recoverySubject = "Password recovery";

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        logger.LogInformation("[User: {user}] Sending confirmation link.", user.Id);

        confirmationLink = UpdateConfirmLink(confirmationLink);

        var content = @$"
            <table>
                <tbody>
                    <tr>
                        <td>
                            <p>Thank you for creating an account with us. To complete your registration and verify your email address, please click button below:</p>
                        </td>
                    </tr

                    <tr>
                      <td style=""padding-top:16px; padding-bottom:16px;"">
                        <div style=""display: inline-block;"">
                          <a href=""{confirmationLink}"" style=""background-color: #EBAB17; color: #F5F5F5; padding: 12px 16px; border-radius: 8px; text-decoration: none; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                            <b>Verify Email Address</b>
                          </a>
                        </div>
                      </td>
                    </tr>
                </tbody>
            </div>
        ";

        var htmlMessage = EmailTemplate(confirmationSubject, content);

        await emailService.SendEmailAsync(email, confirmationSubject, htmlMessage);
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        logger.LogInformation("[User: {user}] Sending password reset code.", user.Id);

        var content = @$"
            <table>
                <tbody>
                    <tr>
                        <td><p>We have received a request to change the password for your account. Please enter code:</p></td>
                    </tr>

                    <tr style=""width: 100%; justify-content: center; margin-top: 16px; margin-bottom: 16px;"">
                        <td><h2 >{resetCode}</h2></td>
                    </tr>
                </tbody>
            </table>
        ";

        var htmlMessage = EmailTemplate(recoverySubject, content);

        await emailService.SendEmailAsync(email, recoverySubject, htmlMessage);
    }

    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        logger.LogInformation("[User: {user}] Sending password reset link.", user.Id);

        resetLink = UpdateResetLink(resetLink);

        var content = @$"
            <table>
                <tbody>
                    <tr>
                        <td><p>We have received a request to change the password for your account. To change password please click button below.</p></td>
                    </tr>

                    <tr style=""width: 100%; justify-content: center; margin-top: 16px; margin-bottom: 16px;"">
                      <td style=""padding-top:16px; padding-bottom:16px;"">
                        <div style=""display: inline-block;"">
                          <a href=""{resetLink}"" style=""background-color: #EBAB17; color: #F5F5F5; padding: 12px 16px; border-radius: 8px; text-decoration: none; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                            <b>Click Here To Change Password</b>
                          </a>
                        </div>
                      </td>
                    </tr>
                </tbody>
            </table>
        ";

        var htmlMessage = EmailTemplate(recoverySubject, content);

        await emailService.SendEmailAsync(email, recoverySubject, htmlMessage);
    }

    public async Task DeleteAccountAsync(string email)
    {
        logger.LogInformation("Sending account deletion confirmation email.");

        var subject = "Account Deletion Confirmation";
        var content = @$"
            <table>
                <tbody>
                    <tr>
                        <td>
                            <p>Your account has been successfully deleted from our system. We're sorry to see you go!</p>
                            <p>If you have any questions or need further assistance, please don't hesitate to contact our support team.</p>
                            <p>Thank you for being a part of our community.</p>
                        </td>
                    </tr>
                </tbody>
            </table>
        ";

        var htmlMessage = EmailTemplate(subject, content);
        await emailService.SendEmailAsync(email, subject, htmlMessage);
    }

    #region Privates

    private string UpdateConfirmLink(string originalLink)
    {
        var domain = emailOptions.CurrentValue.ConfirmationUrl;

        if (string.IsNullOrEmpty(domain))
            return originalLink;

        var paths = new Uri(originalLink).PathAndQuery.Split('?', 2);

        if (paths.Length < 2)
            return originalLink;

        return $"{domain}?{paths[1]}";
    }

    private string UpdateResetLink(string originalLink)
    {
        var domain = emailOptions.CurrentValue.ResetUrl;

        if (string.IsNullOrEmpty(domain))
            return originalLink;

        var paths = new Uri(originalLink).PathAndQuery.Split('?', 2);

        if (paths.Length < 2)
            return originalLink;

        return $"{domain}?{paths[1]}";
    }

    private static string EmailTemplate(string title, string content) => @$"
        <table style=""max-width:512px; margin:auto; font-family: Arial, sans-serif; background-color:#F5F5F5;"">
            <tbody>
                <tr>
                    <td style=""border-bottom:1px solid #EBAB17; padding:0;"">
                        <table>
                            <tbody>
                                <tr>
                                    <td style=""padding:16px; width:64px;"">
                                        <a href=""https://senswave.net"">
                                            <img src=""https://senswave.net/icon.png"" alt=""Logo""
                                                style=""width:48px; border-radius:12px; display:block;"">
                                        </a>
                                    </td>
                                    <td>
                                        <a href=""https://senswave.net"" style=""text-decoration:none;"">
                                            <h1 style=""font-size:24px; color:#33363F; margin:0;"">Senswave</h1>
                                        </a>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td style=""color:#33363F; padding:16px;"">
                        <h3 style=""font-size:18px; margin-top:0; margin-bottom:8px;"">
                            {title}
                        </h3>
                        <div style=""font-size:14px; line-height:1.5;"">
                            {content}
                        </div>
                    </td>
                </tr>

                <tr>
                    <td style=""background-color:#33363F; border-radius:0 12px 0 12px;"">
                        <div style=""padding:16px; font-size:14px;"">
                            <span style=""color:#F5F5F5; margin-right:6px;"">You have a question?</span>
                            <a href=""https://senswave.net/help"" style=""color:#EBAB17; text-decoration:none;"">
                                <b>Check Help</b>
                            </a>
                        </div>
                    </td>
                </tr>

                <tr>
                    <td style=""background-color:#F5F5F5; font-size:12px; color:#888; padding:16px;"">
                        <p style=""margin-top:16px; margin-bottom:8px;"">
                            This email was autogenerated. Please do not reply directly to this message.
                        </p>
                        <p style=""margin-top:0; margin-bottom:8px;"">
                            If you suspect this email is fraudulent or from an illegitimate source, please
                            <a href=""https://senswave.net/contact"" style=""color:#EBAB17;"">contact with us.</a>
                        </p>
                        <p style=""text-align:center; margin:0; padding-top:16px; padding-bottom:16px;"">
                            Senswave 2025
                        </p>
                    </td>
                </tr>
            </tbody>
        </table>
    ";

    #endregion
}
