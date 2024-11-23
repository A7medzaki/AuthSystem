using AuthSystem.Models;
using System.Net.Mail;
using System.Net;

namespace AuthSystem.Helper
{
    public static class EmailSettings
    {
        public static void SendEmail(Email email)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);

            client.EnableSsl = true;

            client.Credentials = new NetworkCredential("a7medzaki345@gmail.com", "ctecazlwvwxyiins");

            client.Send("a7medzaki345@gmail.com", email.To, email.Subject, email.Body);

        }
    }
}
