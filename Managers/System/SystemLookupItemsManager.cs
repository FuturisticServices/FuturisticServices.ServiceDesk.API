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

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ISystemLookupItemsManager
    {
        Task<IEnumerable<LookupItem>> GetItemsAsync();
        Task<LookupItem> GetItemAsync(Enums.LookupItems group);
        Task<LookupItem> GetItemAsync(string groupName);
        Task<LookupItemValue> GetItemAsync(string groupName, string id);
        Task<LookupItem> CreateItemAsync(LookupItem group);
        Task<LookupItem> UpsertGroupAsync(LookupItem group);
    }

    public class SystemLookupItemsManager : SystemBaseManager, ISystemLookupItemsManager
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public SystemLookupItemsManager(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("LookupItems", configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IEnumerable<LookupItem>> GetItemsAsync()
        {
            var query = _container.GetItemLinqQueryable<LookupItem>();
            var iterator = query.ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result;
        }

        public async Task<LookupItem> GetItemAsync(Enums.LookupItems group)
        {
            LookupItem result = await GetItemAsync(group.GetDescription());
            return result;
        }

        public async Task<LookupItem> GetItemAsync(string groupName)
        {
            groupName = groupName.ToTitleCase();

            var query = _container.GetItemLinqQueryable<LookupItem>();
            var iterator = query.Where(x => x.Name == groupName).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<LookupItemValue> GetItemAsync(string groupName, string id)
        {
            LookupItemValue item = null;
            var group = await GetItemAsync(groupName);
            if (group != null) item = await GetItem(group.Values, id);
            return item;
        }

        public async Task<LookupItem> CreateItemAsync(LookupItem group)
        {
            var item = await _container.CreateItemAsync<LookupItem>(group);
            return item;
        }

        public async Task<LookupItem> UpsertGroupAsync(LookupItem group)
        {
            var results = await _container.UpsertItemAsync<LookupItem>(group);
            return results;
        }

        private async Task<LookupItemValue> GetItem(IEnumerable<LookupItemValue> values, string id)
        {
            var value = values.SingleOrDefault(x => x.Id == id);

            //if (value == null && values.Values != null) value = await GetItem(values.Values, id);

            return value;
        }
    }
}
