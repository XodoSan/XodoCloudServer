using Application.Services;
using Application.Services.EmailSenderService;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class EmailSenderServiceTest
    {
        private static string defaultUserEmail = "HodoSan";
        private static string defaultUserPassword = "test";

        private readonly IEmailSender _emailSender;
        private readonly IEmailSenderTools _emailSenderTools = Mock.Of<IEmailSenderTools>(method => method.
            GenereteEmailConfirmLink(It.IsAny<string>()) == $"/{defaultUserEmail}/confirm" && method.
            GeneratePasswordConfirmLink(It.IsAny<string>(), It.IsAny<string>()) == $"/{defaultUserPassword}/confirm");

        public EmailSenderServiceTest()
        {
            _emailSender = new EmailSender();
        }

        [Fact]
        public void SendConfirmEmailAsync_ShouldReturnVoid()
        {
            string emailConfirmLink = _emailSenderTools.GenereteEmailConfirmLink(defaultUserEmail);
            _emailSender.SendEmailAsync(defaultUserEmail, emailConfirmLink);
        }

        [Fact]
        public void SendConfirmPasswordAsync_ShouldReturnVoid()
        {
            string emailHash = HashService.GetHash(defaultUserEmail);
            string passwordHash = HashService.GetHash(defaultUserPassword);

            string passwordConfirmLink = _emailSenderTools.GeneratePasswordConfirmLink(emailHash, passwordHash);
            _emailSender.SendEmailAsync(defaultUserEmail, passwordConfirmLink);
        }
    }
}
