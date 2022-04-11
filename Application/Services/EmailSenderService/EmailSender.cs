﻿using System.Net;
using System.Threading.Tasks;
using System.Net.Mail;
using System;

namespace Application.Services.EmailSenderService
{
    public class EmailSender: IEmailSender
    {
        private static char[] letters = "qwertyuiopasdfghjklzxcvbnm".ToCharArray();

        public async Task SendEmailAsync(string userEmail, string confirmLink)
        {
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

        public string GenereteEmailConfirmLink(string userEmail)
        {
            string baseLink = "https://localhost:5001/api/User/confirm_registration/";
            string dataHash = HashService.GetHash(userEmail);

            string randomWord = GetRandomWord();
            Configuration.randomWord = randomWord;

            return baseLink + dataHash + randomWord;
        }

        public string GeneratePasswordConfirmLink(string userEmailHash, string passwordHash)
        {
            string baseLink = "https://localhost:5001/api/User/confirm_change_password/" + userEmailHash + "/" + passwordHash;

            string randomWord = GetRandomWord();
            Configuration.randomWord = randomWord;

            return baseLink + randomWord;
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
