using Microsoft.Extensions.Options;
using Quartz;
using UserApi.DataAccess;
using UserApi.Models;
using UserApi.Services;

namespace UserApi.Triggers
{
    public class WelcomeEmailTrigger : TriggerBase
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ApplicationDbContext _context;

        public WelcomeEmailTrigger(IOptions<SmtpSettings> smtpSettings, ApplicationDbContext context)
        {
            _smtpSettings = smtpSettings.Value;
            _context = context;
        }

        public override async Task ExecuteAsync(User user)
        {
            // Logic to send the email
            var emailService = new EmailService(_smtpSettings);
            try
            {
                await emailService.SendWelcomeEmail(user.Email, user.FirstName);
                user.HasReceivedEmail = true;

            }
            catch (Exception ex)
            {
             
                user.HasReceivedEmail = false;
                Console.WriteLine(ex.ToString());
            }
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}
