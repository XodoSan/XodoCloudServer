﻿using System.Net;
using System.Threading.Tasks;
using System.Net.Mail;
using System;
using System.Text;

namespace Application.Services.EmailSenderService
{
    public class EmailSender: IEmailSender
    {
        private static readonly char[] letters = "qwertyuiopasdfghjklzxcvbnm".ToCharArray();

        public async Task SendEmailAsync(string userEmail, string confirmLink)
        {
            MailAddress from = new MailAddress(Configuration.emailSender, "HodoCloud");
            MailAddress to = new MailAddress(userEmail);
            MailMessage message = new MailMessage(from, to);
            message.Subject = "HodoCloud auth";

            StringBuilder messageBody = new();
            messageBody
                .Append("Чтобы подтвердить свою почту, перейдите по ссылке: ")
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
            string dataHash = HashService.GetHash(userEmail);

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
            StringBuilder baseLink = new();
            baseLink
                .Append("https://localhost:5001/api/User/confirm_change_password/")
                .Append(userEmailHash)
                .Append("/")
                .Append(passwordHash);

            string randomWord = GetRandomWord();
            Configuration.randomWord = randomWord;
            baseLink.Append(randomWord);

            return baseLink.ToString();
        }

        private static string GetRandomWord()
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
