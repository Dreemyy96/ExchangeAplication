using System.Net;
using System.Net.Mail;

namespace CurrencyExchange.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        public EmailService()
        {

            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("kovalevskijdmitrij314@gmail.com", "qdwppkochhveztdr"),
                EnableSsl = true
            };
        }

        public Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress("kovalevskijdmitrij314@gmail.com", "CurrencyExchange Team");
            var mailMsg = new MailMessage()
            {
                From = fromAddress,
                Subject = subject,
                Body = body
            };
            mailMsg.To.Add(toEmail);
            return _smtpClient.SendMailAsync(mailMsg);
        }
    }
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail,  string subject, string body);
    }
}
