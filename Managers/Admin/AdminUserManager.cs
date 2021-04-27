using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface IAdminUserManager
    {
        Task<IEnumerable<AdminUser>> GetItemsAsync();
        Task<AdminUser> CreateItemAsync(AdminUser user);

}

public class AdminUserManager : AdminBaseManager, IAdminUserManager
    {
        private readonly ISystemManager _systemManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminUserManager(ISystemManager systemManager, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Users", httpContextAccessor, configuration, webHostEnvironment)
        {
            _systemManager = systemManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<AdminUser>> GetItemsAsync()
        {
            var query = _container.GetItemLinqQueryable<AdminUser>();
            var iterator = query.ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result;
        }

        public async Task<AdminUser> CreateItemAsync(AdminUser user)
        {
            var results = await _container.CreateItemAsync<AdminUser>(user, new PartitionKey(user.EmployeeId));
            return results;
        }
    }
}
