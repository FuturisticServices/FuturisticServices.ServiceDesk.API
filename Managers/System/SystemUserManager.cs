using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using TangledServices.ServiceDesk.API.Common;
using TangledServices.ServiceDesk.API.Entities;
using TangledServices.ServiceDesk.API.Managers;

namespace TangledServices.ServiceDesk.API.Managers
{
    public interface ISystemUserManager
    {
        Task<SystemUser> GetItemAsync(string username);
        Task<IEnumerable<SystemUser>> GetItemsAsync();
        Task<Entities.User> CreateItemAsync(Entities.User user);
}

public class SystemUserManager : SystemBaseManager, ISystemUserManager
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public SystemUserManager(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Users", configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<SystemUser> GetItemAsync(string username)
        {
            var query = _container.GetItemLinqQueryable<SystemUser>();
            var iterator = query.Where(x => x.Username.ToLower() == username.ToLower()).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<SystemUser>> GetItemsAsync()
        {
            var query = _container.GetItemLinqQueryable<SystemUser>();
            var iterator = query.ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result;
        }

        public async Task<Entities.User> CreateItemAsync(Entities.User user)
        {
            var results = await _container.CreateItemAsync<Entities.User>(user, new PartitionKey(user.NameLast));
            return results;
        }
    }
}
