using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface IAdminUsersManager
    {
        Task<AdminUser> GetItemAsync(string username);
        Task<IEnumerable<AdminUser>> GetItemsAsync();
        Task<AdminUser> CreateItemAsync(AdminUser adminUser);
    }

public class AdminUsersManager : AdminBaseManager, IAdminUsersManager
    {
        public AdminUsersManager(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Users", httpContextAccessor, configuration, webHostEnvironment)
        { }

        public async Task<AdminUser> GetItemAsync(string username)
        {
            var query = _container.GetItemLinqQueryable<AdminUser>();
            var iterator = query.Where(x => x.Username.ToLower() == username.ToLower()).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<AdminUser>> GetItemsAsync()
        {
            var query = _container.GetItemLinqQueryable<AdminUser>();
            var iterator = query.ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result;
        }

        public async Task<AdminUser> CreateItemAsync(AdminUser adminUser)
        {
            var results = await _container.CreateItemAsync<AdminUser>(adminUser, new PartitionKey(adminUser.EmployeeId));
            return results;
        }
    }
}
