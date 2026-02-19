using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace CinemaECommerce.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials= new NetworkCredential("msabr0306@gmail.com", "fsrs reyp kuue fchy")
            };
            return client.SendMailAsync(
                new MailMessage
                (from: "msabr0306@gmail.com",
                to :email,
                subject,
                htmlMessage
                )
                {
                   IsBodyHtml = true
                }                           );
        }
    }
}
