using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ITenantUserManager
    {
        Task<IEnumerable<Entities.User>> GetItemsAsync();
        Task<Entities.User> CreateItemAsync(Entities.User user);

}

public class TenantUserManager : TenantBaseManager, ITenantUserManager
    {
        internal ISystemTenantsManager _systemTenantsService;
        internal IHttpContextAccessor _httpContextAccessor;
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public TenantUserManager(ISystemTenantsManager systemTenantsService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Users", systemTenantsService, httpContextAccessor, configuration, webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<Entities.User>> GetItemsAsync()
        {
            var query = _container.GetItemLinqQueryable<Entities.User>();
            var iterator = query.ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result;
        }

        public async Task<Entities.User> CreateItemAsync(Entities.User user)
        {
            var results = await _container.CreateItemAsync<Entities.User>(user, new PartitionKey(user.EmployeeID));
            return results;
        }
    }
}
