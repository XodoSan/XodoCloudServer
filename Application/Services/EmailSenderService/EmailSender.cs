using System.Net;
using System.Threading.Tasks;
using System.Net.Mail;
using System;

namespace Application.Services.EmailSenderService
{
    public class EmailSender
    {
        private static char[] letters = "qwertyuiopasdfghjklzxcvbnm".ToCharArray();

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

        private static string GenereteConfirmLink(string userData)
        {
            string baseLink = "https://localhost:5001/api/User/confirm_registration/" + userData + "/";
            string dataHash = HashService.GetHash(userData);

            string randomWord = GetRandomWord();
            Configuration.randomWord = randomWord;

            return baseLink + dataHash + randomWord;
        }

        private static string GetRandomWord()
        {
            Random random = new();
            int numLetters = random.Next(5, 10);
            string randWord = "";

            for (int i = 0; i < numLetters; i++)
            {
                int randomIndex = random.Next(0, letters.Length - 1);
                randWord += letters[randomIndex];
            }

            return randWord;
        }
    }
}
