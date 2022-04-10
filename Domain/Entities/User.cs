namespace Domain.Entities
{
    public class User
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public void UpdatePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }
    }
}
