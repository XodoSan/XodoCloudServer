namespace Application.Services.EmailSenderService
{
    public interface IEmailSenderTools
    {
        public string GenereteEmailConfirmLink(string userEmail);
        public string GeneratePasswordConfirmLink(string userEmailHash, string passwordHash);
        public string GetRandomWord();
    }
}
