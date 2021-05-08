using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ISystemUsersManager
    {
        Task<SystemAuthenticateUser> GetItemAsync(string username);
        Task<SystemAuthenticateUser> GetItemAsync(Guid id);
        Task<IEnumerable<SystemAuthenticateUser>> GetItemsAsync();
        //Task<SystemUser> CreateItemAsync(SystemUser user);
        Task<SystemUser> UpsertItemAsync(SystemUser user);
        Task<SystemAuthenticateUser> CreateItemAsync(SystemAuthenticateUser user);
        Task<SystemAuthenticateUser> UpsertItemAsync(SystemAuthenticateUser user);
        //Task<TransactionalBatchResponse> ResetUsernameAsync(string sourceUserName, SystemAuthenticateUser user);
        Task DeleteItemAsync(SystemAuthenticateUser user);
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

        public async Task<SystemAuthenticateUser> GetItemAsync(string username)
        {
            var query = _container.GetItemLinqQueryable<SystemAuthenticateUser>();
            var iterator = query.Where(x => x.Username.ToLower() == username.ToLower()).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<SystemAuthenticateUser>> GetItemsAsync()
        {
            var query = _container.GetItemLinqQueryable<SystemAuthenticateUser>();
            var iterator = query.ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result;
        }

        public async Task<SystemAuthenticateUser> GetItemAsync(Guid id)
        {
            var query = _container.GetItemLinqQueryable<SystemAuthenticateUser>();
            var iterator = query.Where(x => x.Id == id.ToString()).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        //public async Task<SystemUser> CreateItemAsync(SystemUser user)
        //{
        //    var results = await _container.CreateItemAsync<SystemUser>(user, new PartitionKey(user.Username));
        //    return results;
        //}

        public async Task<SystemUser> UpsertItemAsync(SystemUser user)
        {
            var results = await _container.UpsertItemAsync<SystemUser>(user);
            return results;
        }

        public async Task<SystemAuthenticateUser> CreateItemAsync(SystemAuthenticateUser user)
        {
            var results = await _container.CreateItemAsync<SystemAuthenticateUser>(user, new PartitionKey(user.Username));
            return results;
        }

        public async Task<SystemAuthenticateUser> UpsertItemAsync(SystemAuthenticateUser user)
        {
            var results = await _container.UpsertItemAsync<SystemAuthenticateUser>(user);
            return results;
        }

        //public async Task<TransactionalBatchResponse> ResetUsernameAsync(string sourceUserName, SystemAuthenticateUser user)
        //{
        //    //  All or nothing!
        //    //  NoSQL databases do not support updating the partition key value of an existing item. Need to delete entire document and create new.
        //    //  https://devblogs.microsoft.com/cosmosdb/introducing-transactionalbatch-in-the-net-sdk/
        //    var response = await _container.CreateTransactionalBatch(new PartitionKey(sourceUserName))
        //      .DeleteItem(user.Id)
        //      .ExecuteAsync();

        //    return response;
        //}

        public async Task DeleteItemAsync(SystemAuthenticateUser user)
        {
            await _container.DeleteItemAsync<SystemAuthenticateUser>(user.Id, new PartitionKey(user.Username));
        }
    }
}
