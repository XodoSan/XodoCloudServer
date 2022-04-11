using System.Threading.Tasks;

namespace Application.Services.EmailSenderService
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string userEmail, string confirmLink);
        public string GenereteEmailConfirmLink(string userEmail);
        public string GeneratePasswordConfirmLink(string userEmailHash, string passwordHash);
    }
}
