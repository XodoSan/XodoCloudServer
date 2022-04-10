using System.Net;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Application.Services.EmailSenderService
{
    public class EmailSender
    {
        public static async Task SendEmailAsync(string userEmail)
        {
            string confirmLink = GenereteConfirmLink(userEmail);

            MailAddress from = new MailAddress(Configuration.emailSender, "HodoCloud");
            MailAddress to = new MailAddress(userEmail);
            MailMessage m = new MailMessage(from, to);
            m.Subject = "HodoCloud auth";
            m.Body = "Чтобы подтвердить свою почту, перейдите по ссылке: " + '\u0022' + confirmLink + '\u0022' + @"\";
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(Configuration.emailSender, Configuration.userPassword);
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(m);
        }

        public static string GenereteConfirmLink(string userEmail)
        {
            string baseLink = "https://localhost:5001/api/User/confirm_registration/" + userEmail + "/";
            string emailHash = HashService.GetHash(userEmail);

            return baseLink + emailHash;
        }
    }
}
