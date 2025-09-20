using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ModuWeb.Api.Services
{
    public class SmtpEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly string _fromAddress;

        public SmtpEmailService(string smtpHost, int smtpPort, string smtpUser, string smtpPass, string fromAddress)
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPass = smtpPass;
            _fromAddress = fromAddress;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using (var client = new SmtpClient(_smtpHost, _smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_smtpUser, _smtpPass);

                var mail = new MailMessage(_fromAddress, to, subject, body);
                mail.IsBodyHtml = true;
                await client.SendMailAsync(mail);
            }
        }
    }
}
