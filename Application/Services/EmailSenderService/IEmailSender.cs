using System.Threading.Tasks;

namespace Application.Services.EmailSenderService
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string userEmail, string confirmLink);
    }
}
