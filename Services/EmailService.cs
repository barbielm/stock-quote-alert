using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace stock_alert_quote.Services
{
    public class EmailService
    {
        public static void SendEmail(string userEmail, string password, string recipientEmail, string message, string subject)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(userEmail));
            email.To.Add(MailboxAddress.Parse(recipientEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Plain) { Text = message };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(userEmail, password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
