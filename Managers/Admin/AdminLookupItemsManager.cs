using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface IAdminLookupItemsManager
    {
        Task<IEnumerable<LookupItem>> GetItemsAsync();
        Task<LookupItem> GetItemAsync(Enums.LookupItems entity);
        Task<LookupItem> GetItemAsync(string name);
        Task<LookupItemValue> GetItemAsync(string name, string id);
        Task<LookupItem> CreateItemAsync(LookupItem entity);
        Task<LookupItem> UpsertGroupAsync(LookupItem entity);
    }

    public class AdminLookupItemsManager : AdminBaseManager, IAdminLookupItemsManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminLookupItemsManager(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("LookupItems", httpContextAccessor, configuration, webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<LookupItem> GetItemAsync(Enums.LookupItems entity)
        {
            LookupItem result = await GetItemAsync(entity.GetDescription());
            return result;
        }

        public async Task<LookupItem> GetItemAsync(string name)
        {
            name = name.ToTitleCase();

            var query = _container.GetItemLinqQueryable<LookupItem>();
            var iterator = query.Where(x => x.Name == name).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<LookupItemValue> GetItemAsync(string name, string id)
        {
            LookupItemValue item = null;
            var group = await GetItemAsync(name);
            if (group != null) item = await GetItem(group.Values, id);
            return item;
        }

        public async Task<LookupItem> CreateItemAsync(LookupItem entity)
        {
            var item = await _container.CreateItemAsync<LookupItem>(entity);
            return item;
        }

        public async Task<LookupItem> UpsertGroupAsync(LookupItem entity)
        {
            var results = await _container.UpsertItemAsync<LookupItem>(entity);
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
