using System.Net;
using System.Net.Mail;
using IdentityServer.Models;
using Microsoft.Extensions.Options;

#nullable disable

namespace IdentityServer.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}

public class EmailSender : IEmailSender
{
    private readonly MailSettings _mailSettings;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(
        IOptions<MailSettings> mailSettings,
        ILogger<EmailSender> logger)
    {
        _mailSettings = mailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var mailRequest = new MailRequest
        {
            Subject = subject,
            ToEmail = email,
            Body = message,
        };

        try
        {
            await SendEmail(mailRequest);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Send Email Error:{ex.Message}");
        }
    }
    private async Task SendEmail(MailRequest mailRequest)
    {
        MailMessage message = new MailMessage();
        SmtpClient smtp = new SmtpClient();
        message.From = new MailAddress(_mailSettings.Mail, _mailSettings.DisplayName);
        message.To.Add(new MailAddress(mailRequest.ToEmail));
        message.Subject = mailRequest.Subject;
        if (mailRequest.Attachments != null)
        {
            foreach (var file in mailRequest.Attachments)
            {
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        Attachment att = new Attachment(new MemoryStream(fileBytes), file.FileName);
                        message.Attachments.Add(att);
                    }
                }
            }
        }
        message.IsBodyHtml = false;
        message.Body = mailRequest.Body;
        smtp.Port = _mailSettings.Port;
        smtp.Host = _mailSettings.Host;
        smtp.EnableSsl = true;
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        await smtp.SendMailAsync(message);
    }
}
