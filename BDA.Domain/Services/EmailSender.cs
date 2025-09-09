using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BDA.Data;
using BDA.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace BDA.Services
{
    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseMock { get; set; }
    }

    public interface IEmailSender : Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
    {
        Task SendEmailAsync(MailMessage mailMessage);
    }

    public class EmailSender : IEmailSender //Microsoft.AspNetCore.Identity.UI.Services.IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly IHostingEnvironment _env;
        private readonly BdaDBContext _context;

        // Get our parameterized configuration
        public EmailSender(IOptions<EmailSettings> emailSettings, BdaDBContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
            _emailSettings = emailSettings.Value;
        }

        // Use our configuration to send the email by using SmtpClient
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (_emailSettings.UseMock)
                return Task.CompletedTask;
            
            var msg = new MailMessage(
                new MailAddress(_emailSettings.FromAddress, _emailSettings.FromName),
                new MailAddress(email)
            )
            {
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };


            return SendEmailAsync(msg);
        }

        public Task SendEmailAsync(MailMessage mailMessage)
        {
            if (_emailSettings.UseMock)
                return Task.CompletedTask;
            
            var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                EnableSsl = _emailSettings.EnableSsl,
            };

            if (!String.IsNullOrEmpty(_emailSettings.Username))
            {
                client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            }

            mailMessage.From = new MailAddress(_emailSettings.FromAddress, _emailSettings.FromName);

            return client.SendMailAsync(mailMessage);
        }
    }

    public class MockEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
         
            Console.WriteLine("Sending email ->");
            Console.WriteLine("  To: " + email);
            Console.WriteLine("  Subject: " + subject);
            Console.WriteLine("  Body: " + htmlMessage);

            return Task.CompletedTask;
        }

        public Task SendEmailAsync(MailMessage mailMessage)
        {
            Console.WriteLine("Sending email ->");
            Console.WriteLine("  To: " + String.Join(", ", mailMessage.To.Select(x => x.Address + " <" + x.DisplayName + ">")));
            Console.WriteLine("  Subject: " + mailMessage.Subject);
            Console.WriteLine("  Body: " + mailMessage.Body);

            return Task.CompletedTask;
        }
    }
}
