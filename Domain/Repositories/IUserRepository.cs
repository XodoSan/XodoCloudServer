using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        public void AddUser(User user);
        public List<User> GetAllUsers();
    }
}