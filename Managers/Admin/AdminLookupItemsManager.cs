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
        Task<IEnumerable<AdminLookupItem>> GetItemsAsync();
        Task<AdminLookupItem> GetItemAsync(Enums.LookupItems entity);
        Task<AdminLookupItem> GetItemAsync(string name);
        Task<AdminLookupItemValue> GetItemAsync(string name, string id);
        Task<AdminLookupItem> CreateItemAsync(AdminLookupItem entity);
        Task<AdminLookupItem> UpsertGroupAsync(AdminLookupItem entity);
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
        public async Task<IEnumerable<AdminLookupItem>> GetItemsAsync()
        {
            var query = _container.GetItemLinqQueryable<AdminLookupItem>();
            var iterator = query.ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result;
        }

        public async Task<AdminLookupItem> GetItemAsync(Enums.LookupItems entity)
        {
            AdminLookupItem result = await GetItemAsync(entity.GetDescription());
            return result;
        }

        public async Task<AdminLookupItem> GetItemAsync(string name)
        {
            name = name.ToTitleCase();

            var query = _container.GetItemLinqQueryable<AdminLookupItem>();
            var iterator = query.Where(x => x.Name == name).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<AdminLookupItemValue> GetItemAsync(string name, string id)
        {
            AdminLookupItemValue item = null;
            var group = await GetItemAsync(name);
            if (group != null) item = await GetItem(group.Values, id);
            return item;
        }

        public async Task<AdminLookupItem> CreateItemAsync(AdminLookupItem entity)
        {
            var item = await _container.CreateItemAsync<AdminLookupItem>(entity);
            return item;
        }

        public async Task<AdminLookupItem> UpsertGroupAsync(AdminLookupItem entity)
        {
            var results = await _container.UpsertItemAsync<AdminLookupItem>(entity);
            return results;
        }

        private async Task<AdminLookupItemValue> GetItem(IEnumerable<AdminLookupItemValue> values, string id)
        {
            var value = values.SingleOrDefault(x => x.Id == id);

            //if (value == null && values.Values != null) value = await GetItem(values.Values, id);

            return value;
        }
    }
}
