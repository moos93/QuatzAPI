using System.Net.Mail;
using System.Net;
using UserApi.Models;

namespace UserApi.Services
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public async Task SendWelcomeEmail(string email, string firstName)
        {
            using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
            {
                // Enable SSL or TLS
                client.EnableSsl = true;

                // Credentials
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);

                // Create the mail message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.Username),
                    Subject = "Welcome!",
                    Body = $"Hello {firstName}, welcome to our platform!",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);
                await client.SendMailAsync(mailMessage);
            }
        }

    }
}
