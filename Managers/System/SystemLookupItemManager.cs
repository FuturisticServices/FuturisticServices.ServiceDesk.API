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
using TangledServices.ServicePortal.API.Extensions;
using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ISystemLookupItemManager
    {
        Task<IEnumerable<LookupGroupEntity>> GetItemsAsync();
        Task<LookupGroupEntity> GetItemAsync(string groupName);
        Task<LookupItemEntity> GetItemAsync(string groupName, string id);
        Task<LookupGroupEntity> CreateItemAsync(LookupGroupEntity group);
        Task<LookupGroupEntity> UpsertGroupAsync(LookupGroupEntity group);
    }

    public class SystemLookupItemManager : SystemBaseManager, ISystemLookupItemManager
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public SystemLookupItemManager(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("LookupItems", configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IEnumerable<LookupGroupEntity>> GetItemsAsync()
        {
            var query = _container.GetItemLinqQueryable<LookupGroupEntity>();
            var iterator = query.ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result;
        }

        public async Task<LookupGroupEntity> GetItemAsync(string groupName)
        {
            groupName = groupName.ToTitleCase();

            var query = _container.GetItemLinqQueryable<LookupGroupEntity>();
            var iterator = query.Where(x => x.Group == groupName).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<LookupItemEntity> GetItemAsync(string groupName, string id)
        {
            LookupItemEntity item = null;
            var group = await GetItemAsync(groupName);
            if (group != null) item = await GetItem(group.Items, id);
            return item;
        }

        public async Task<LookupGroupEntity> CreateItemAsync(LookupGroupEntity group)
        {
            var results = await _container.CreateItemAsync<LookupGroupEntity>(group);
            return results;
        }

        public async Task<LookupGroupEntity> UpsertGroupAsync(LookupGroupEntity group)
        {
            var results = await _container.UpsertItemAsync<LookupGroupEntity>(group);
            return results;
        }

        private async Task<LookupItemEntity> GetItem(IEnumerable<LookupItemEntity> items, string id)
        {
            var item = items.SingleOrDefault(x => x.Id == id);

            if (item == null && item.Items != null) item = await GetItem(item.Items, id);

            return item;
        }
    }
}
