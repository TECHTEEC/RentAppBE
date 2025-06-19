
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace RentAppBE.Repositories.SenderService.EmailService
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
        {
            var host = _config["EmailSettings:Host"];
            var port = int.Parse(_config["EmailSettings:Port"]);
            var username = _config["EmailSettings:Username"];
            var password = _config["EmailSettings:Password"];
            var fromEmail = _config["EmailSettings:FromEmail"];
            var fromName = _config["EmailSettings:FromName"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = bodyHtml };
            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            try
            {
                // ✅ Use SslOnConnect for port 465
                smtp.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                await smtp.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(username, password);
                await smtp.SendAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("MailKit error: " + ex.Message);
                throw;
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
