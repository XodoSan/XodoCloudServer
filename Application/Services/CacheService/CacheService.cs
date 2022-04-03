using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Application.Services.CacheService
{
    public class CacheService: ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IUserRepository _userRepository;

        public CacheService(IMemoryCache cache, IUserRepository userRepository)
        {
            _cache = cache;
            _userRepository = userRepository;
        }

        public void SetAddedUserCache(User user)
        {
            _cache.Set(user.Email, user, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        }

        public async Task<User> GetUser(string userEmail)
        {
            User user = null;
            if (!_cache.TryGetValue(userEmail, out user))
            {
                user = await _userRepository.GetUserByEmail(user.Email);
                if (user != null)
                {
                    _cache.Set(user.Email, user,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }

            return user;
        }
    }
}
