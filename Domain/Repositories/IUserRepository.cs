using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        public void AddUser(User user);
        public List<User> GetAllUsers();
        public Task<User> GetUserByEmail(string email);
    }
}