using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ISystemUsersManager
    {
        Task<SystemUser> GetItemAsync(string username);
        Task<IEnumerable<SystemUser>> GetItemsAsync();
        Task<SystemUser> CreateItemAsync(SystemUser user);
}

public class SystemUsersManager : SystemBaseManager, ISystemUsersManager
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public SystemUsersManager(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Users", configuration, webHostEnvironment)
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

        public async Task<SystemUser> CreateItemAsync(SystemUser user)
        {
            var results = await _container.CreateItemAsync<SystemUser>(user, new PartitionKey(user.EmployeeId));
            return results;
        }
    }
}
