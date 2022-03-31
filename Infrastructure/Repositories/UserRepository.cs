using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<User> GetByEmail(string email)
        {
            return await _context.Set<User>().FirstOrDefaultAsync(user => user.Email == email);
        }
    }
}
