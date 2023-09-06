using AspNetCoreIdentityApp.Core.OptionsModels;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendResetPasswordEmail(string resetPasswordEmailLink, string ToEmail)
        {

            //Simple Mail Transfer Protokol
            var smtpClient = new SmtpClient
            {
                Host = _settings.Host,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Port = 587,
                Credentials = new NetworkCredential(_settings.Email, _settings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_settings.Email);
            mailMessage.To.Add(ToEmail);

            mailMessage.Subject = "Localhost | Şifre Sıfırlama Linki";

            mailMessage.Body = $"<h4> Şifrenizi yenilemek için aşağıdaki linke tıklayınız. </h4> <p><a href='{resetPasswordEmailLink}'>Şifre Yenileme Linki</a></p>";

            mailMessage.IsBodyHtml = true;
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
