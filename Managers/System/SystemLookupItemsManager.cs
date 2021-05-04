using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ISystemLookupItemsManager
    {
        Task<IEnumerable<SystemLookupItem>> GetItemsAsync();
        Task<SystemLookupItem> GetItemAsync(Enums.LookupItems group);
        Task<SystemLookupItem> GetItemAsync(string groupName);
        Task<SystemLookupItemValue> GetItemAsync(string groupName, string id);
        Task<SystemLookupItem> CreateItemAsync(SystemLookupItem group);
        Task<SystemLookupItem> UpsertGroupAsync(SystemLookupItem group);
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
        public async Task<IEnumerable<SystemLookupItem>> GetItemsAsync()
        {
            var query = _container.GetItemLinqQueryable<SystemLookupItem>();
            var iterator = query.ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result;
        }

        public async Task<SystemLookupItem> GetItemAsync(Enums.LookupItems group)
        {
            SystemLookupItem result = await GetItemAsync(group.GetDescription());
            return result;
        }

        public async Task<SystemLookupItem> GetItemAsync(string groupName)
        {
            groupName = groupName.ToTitleCase();

            var query = _container.GetItemLinqQueryable<SystemLookupItem>();
            var iterator = query.Where(x => x.Name == groupName).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<SystemLookupItemValue> GetItemAsync(string groupName, string id)
        {
            SystemLookupItemValue item = null;
            var group = await GetItemAsync(groupName);
            if (group != null) item = await GetItem(group.Values, id);
            return item;
        }

        public async Task<SystemLookupItem> CreateItemAsync(SystemLookupItem group)
        {
            var item = await _container.CreateItemAsync<SystemLookupItem>(group);
            return item;
        }

        public async Task<SystemLookupItem> UpsertGroupAsync(SystemLookupItem group)
        {
            var results = await _container.UpsertItemAsync<SystemLookupItem>(group);
            return results;
        }

        private async Task<SystemLookupItemValue> GetItem(IEnumerable<SystemLookupItemValue> values, string id)
        {
            var value = values.SingleOrDefault(x => x.Id == id);

            //if (value == null && values.Values != null) value = await GetItem(values.Values, id);

            return value;
        }
    }
}
