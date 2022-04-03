using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Services.CacheService
{
    public interface ICacheService
    {
        public void SetAddedUserCache(User user);
        public Task<User> GetUser(string userEmail);
    }
}
