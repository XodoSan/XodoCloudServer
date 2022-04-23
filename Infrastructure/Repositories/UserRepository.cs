using Domain.Entities;
using Domain.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class UserRepository: IUserRepository
    {
        private AppDBContext _context;

        public UserRepository(AppDBContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
            _context.Set<User>().Add(user);
        }

        public List<User> GetAllUsers()
        {
            return _context.Set<User>().ToList(); 
        }

        public User GetUserByEmail(string email)
        {
            return _context.Set<User>().FirstOrDefault(user => user.Email == email);
        }
    }
}
