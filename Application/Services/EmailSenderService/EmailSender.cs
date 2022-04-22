using System.Net;
using System.Threading.Tasks;
using System.Net.Mail;
using System;
using System.Text;
using Application.Services.HashService;

namespace Application.Services.EmailSenderService
{
    public class EmailSender: IEmailSender, IEmailSenderTools
    {
        public static string stubData; //this stub variable created for tests
        private static readonly char[] letters = "qwertyuiopasdfghjklzxcvbnm".ToCharArray();

        private readonly IHashService _hashService;

        public EmailSender(IHashService hashService)
        {
            _hashService = hashService;
        }

        public async Task SendEmailAsync(string userEmail, string confirmLink)
        {
            stubData = userEmail;

            MailAddress from = new MailAddress(Configuration.emailSender, "HodoCloud");
            MailAddress to = new MailAddress(userEmail);
            MailMessage message = new MailMessage(from, to);
            message.Subject = "HodoCloud auth";

            StringBuilder messageBody = new();
            messageBody
                .Append("Чтобы подтвердить свою личность, перейдите по ссылке: ")
                .Append('\u0022')
                .Append(confirmLink)
                .Append('\u0022')
                .Append("/");

            message.Body = messageBody.ToString();
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(Configuration.emailSender, Configuration.userPassword);
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(message);
        }

        public string GenereteEmailConfirmLink(string userEmail)
        {
            string baseLink = "https://localhost:5001/api/User/confirm_registration/";
            string dataHash = _hashService.GetHash(userEmail);

            string randomWord = GetRandomWord();
            Configuration.randomWord = randomWord;

            StringBuilder confirmLink = new();
            confirmLink
                .Append(baseLink)
                .Append(dataHash)
                .Append(randomWord);

            return confirmLink.ToString();
        }

        public string GeneratePasswordConfirmLink(string userEmailHash, string passwordHash)
        {
            StringBuilder confirmLink = new();
            confirmLink
                .Append("https://localhost:5001/api/User/confirm_change_password/")
                .Append(userEmailHash)
                .Append("/")
                .Append(passwordHash);

            string randomWord = GetRandomWord();
            Configuration.randomWord = randomWord;
            Configuration.userPasswordHash = passwordHash;
            confirmLink.Append(randomWord);

            return confirmLink.ToString();
        }

        public string GetRandomWord()
        {
            Random random = new();
            int numLetters = random.Next(5, 10);
            StringBuilder randWord = new();

            for (int i = 0; i < numLetters; i++)
            {
                int randomIndex = random.Next(0, letters.Length - 1);
                randWord.Append(letters[randomIndex]);
            }

            return randWord.ToString();
        }
    }
}
