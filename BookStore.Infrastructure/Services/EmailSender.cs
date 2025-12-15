using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;  
namespace BookStore.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly int _smtpPort;
        private readonly string _fromEmail;

        public EmailSender(IConfiguration configuration)
        {
            _smtpServer = configuration["EmailSettings:SmtpServer"] ?? throw new ArgumentNullException(nameof(_smtpServer));
            _smtpUser = configuration["EmailSettings:SmtpUser"] ?? throw new ArgumentNullException(nameof(_smtpUser));
            _smtpPass = configuration["EmailSettings:SmtpPassword"] ?? throw new ArgumentNullException(nameof(_smtpPass));
            _smtpPort = int.TryParse(configuration["EmailSettings:SmtpPort"], out var port) ? port : throw new ArgumentNullException(nameof(_smtpPort));
            _fromEmail = configuration["EmailSettings:FromEmail"] ?? throw new ArgumentNullException(nameof(_smtpServer));
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpClient = new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);

        }
    }
}
