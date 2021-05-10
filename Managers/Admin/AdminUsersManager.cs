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
    public interface IAdminUsersManager
    {
        Task<AdminAuthenticateUser> GetItemAsync(string username);
        Task<AdminAuthenticateUser> GetItemAsync(Guid id);
        Task<IEnumerable<AdminUser>> GetItemsAsync();
        Task<AdminAuthenticateUser> CreateItemAsync(AdminAuthenticateUser adminUser);
        Task<AdminAuthenticateUser> UpsertItemAsync(AdminAuthenticateUser user);
        Task DeleteItemAsync(AdminAuthenticateUser user);
    }

    public class AdminUsersManager : AdminBaseManager, IAdminUsersManager
    {
        #region Public methods
        public AdminUsersManager(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Users", httpContextAccessor, configuration, webHostEnvironment)
        { }

        public async Task<AdminAuthenticateUser> GetItemAsync(string username)
        {
            var query = _container.GetItemLinqQueryable<AdminAuthenticateUser>();
            var iterator = query.Where(x => x.Username.ToLower() == username.ToLower()).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<AdminAuthenticateUser> GetItemAsync(Guid id)
        {
            var query = _container.GetItemLinqQueryable<AdminAuthenticateUser>();
            var iterator = query.Where(x => x.Id == id.ToString()).ToFeedIterator();
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

        public async Task<AdminAuthenticateUser> CreateItemAsync(AdminAuthenticateUser adminAuthenticateUser)
        {
            var results = await _container.CreateItemAsync<AdminAuthenticateUser>(adminAuthenticateUser, new PartitionKey(adminAuthenticateUser.Username));
            return results;
        }

        public async Task<AdminAuthenticateUser> UpsertItemAsync(AdminAuthenticateUser user)
        {
            var results = await _container.UpsertItemAsync<AdminAuthenticateUser>(user);
            return results;
        }

        public async Task DeleteItemAsync(AdminAuthenticateUser user)
        {
            await _container.DeleteItemAsync<AdminAuthenticateUser>(user.Id, new PartitionKey(user.Username));
        }
    }
    #endregion Public methods
}
