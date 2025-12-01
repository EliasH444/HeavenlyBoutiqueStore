using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace learning.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtp = new SmtpClient
            {
                Host = _config["SMTP:Host"],
                Port = int.Parse(_config["SMTP:Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _config["SMTP:Username"],
                    _config["SMTP:Password"])
            };

            var message = new MailMessage
            {
                From = new MailAddress(_config["SMTP:Username"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);
            await smtp.SendMailAsync(message);
        }
    }
}
