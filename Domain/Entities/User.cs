namespace Domain.Entities
{
    public class User
    {
        public User(int id, string email, string passwordHash)
        {
            Id = id;
            Email = email;
            PasswordHash = passwordHash;
        }

        public int Id { get; set; }
        public string Email { get; protected set; }
        public string PasswordHash { get; protected set; }
    }
}
