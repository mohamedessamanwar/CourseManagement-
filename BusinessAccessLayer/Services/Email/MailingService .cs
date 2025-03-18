using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace BusinessAccessLayer.Services.Email


{
    public class MailingService : IMailingService
    {
        private readonly MailSetting mailSetting;

        public MailingService(IOptions<MailSetting> _mailSetting)
        {
            mailSetting = _mailSetting.Value;
        }
        public async Task<bool> SendEmailAsync(string email, string subject, string body)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(mailSetting.Email);
            message.Subject = subject;
            message.Body = $"<html><body>{body}</body></html>";
            message.IsBodyHtml = true;
            message.To.Add(email);

            var smtpClient = new SmtpClient(mailSetting.Host)
            {
                Port = mailSetting.Port,
                Credentials = new NetworkCredential(mailSetting.Email, mailSetting.Password),
                EnableSsl = mailSetting.EnableSsl,
            };

            try
            {
                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}
