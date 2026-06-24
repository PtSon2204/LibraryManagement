using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.EmailDTOs;
using LibraryManagement.Business.Interfaces;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LibraryManagement.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(
            IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));

            message.To.Add(MailboxAddress.Parse(to));

            message.Subject = subject;

            message.Body = new TextPart("html")
                {
                    Text = body
                };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _settings.SmtpServer,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _settings.SenderEmail,
                _settings.SenderPassword);

            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
    }
}
