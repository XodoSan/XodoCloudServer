using Application.Services.EmailSenderService;
using Application.Services.HashService;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class EmailSenderServiceTests
    {
        private static string defaultUserEmail = "HodoSan";
        private static string defaultUserPassword = "test";

        private readonly IEmailSender _emailSender;
        private readonly IEmailSenderTools _emailSenderTools = Mock.Of<IEmailSenderTools>(method => method.
            GenereteEmailConfirmLink(It.IsAny<string>()) == $"/{defaultUserEmail}/confirm" && method.
            GeneratePasswordConfirmLink(It.IsAny<string>(), It.IsAny<string>()) == $"/{defaultUserPassword}/confirm");

        private readonly IHashService _hashService = Mock.Of<IHashService>();

        public EmailSenderServiceTests()
        {
            _emailSender = new EmailSender(_hashService);
        }

        [Fact]
        public void SendConfirmEmailAsync_Test()
        {
            string emailConfirmLink = _emailSenderTools.GenereteEmailConfirmLink(defaultUserEmail);

            _emailSender.SendEmailAsync(defaultUserEmail, emailConfirmLink);

            Assert.Equal(defaultUserEmail, EmailSender.stubData);
        }

        [Fact]
        public void SendConfirmPasswordAsync_Test()
        {
            string emailHash = _hashService.GetHash(defaultUserEmail);
            string passwordHash = _hashService.GetHash(defaultUserPassword);

            string passwordConfirmLink = _emailSenderTools.GeneratePasswordConfirmLink(emailHash, passwordHash);
            _emailSender.SendEmailAsync(defaultUserEmail, passwordConfirmLink);

            Assert.Equal(defaultUserEmail, EmailSender.stubData);
        }
    }
}
