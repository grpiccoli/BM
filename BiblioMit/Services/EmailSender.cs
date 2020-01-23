using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Configuration;

namespace BiblioMit.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkId=532713
    public class EmailSender : IEmailSender
    {
        public EmailSender(
            IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Configuration.GetValue<string>("SG_KEY"), subject, message, email);
        }

        public static Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("no-responder@email.bibliomit.cl", "BiblioMit"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }
    }
}
