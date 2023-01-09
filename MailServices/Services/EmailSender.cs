using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;

namespace MailServices.Services
{
    public class EmailSender : IEmailSender
    {

        private string _myUsername;
        private string _myPwd;
        private string _appName;

        public EmailSender(IConfiguration configuration)
        {
            _myUsername = configuration["Email:Username"];
            _myPwd = configuration["Email:Pwd"];
            _appName = configuration["Email:AppName"];
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_appName, "noreply@myapp.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = htmlMessage };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate(_myUsername, _myPwd);
                await client.SendAsync(emailMessage);
                client.Disconnect(true);
            }
        }
    }
}
