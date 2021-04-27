using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Managers.System;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ITenantLookupItemManager
    {
        Task<LookupGroup> GetItemAsync(string groupName);
        Task<LookupGroup> GetItemAsync(string groupName, string itemName);
        Task<LookupGroup> GetItemAsync(string groupName, Guid id);
        Task<IEnumerable<LookupGroup>> GetItemsAsync();
        Task<LookupGroup> CreateItemAsync(LookupGroup lookupGroup);

}

public class TenantLookupItemManager : TenantBaseManager, ITenantLookupItemManager
    {
        internal ICustomerManager _customerManager;
        internal IHttpContextAccessor _httpContextAccessor;
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public TenantLookupItemManager(ICustomerManager customerManager, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("LookupItems", customerManager, httpContextAccessor, configuration, webHostEnvironment)
        {
            _customerManager = customerManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<LookupGroup> GetItemAsync(string groupName)
        {
            Enums.LookupGroups lookupGroup;

            groupName = groupName.ToCamelCase();
            if (Enum.TryParse<Enums.LookupGroups>(groupName, true, out lookupGroup))
            {
                groupName = lookupGroup.GetDescription().ToCamelCase();

                QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.lookupName = @groupName")
                .WithParameter("@groupName", groupName);

                LookupGroup result = new LookupGroup();
                using (FeedIterator<LookupGroup> feedIterator = _container.GetItemQueryIterator<LookupGroup>(query))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        FeedResponse<LookupGroup> response = await feedIterator.ReadNextAsync();
                        foreach (var item in response)
                        {
                            result = item;
                        }
                    }
                }

                return result;
            }

            return null;
        }

        public async Task<LookupGroup> GetItemAsync(string groupName, string itemName)
        {
            var query = _container.GetItemLinqQueryable<LookupGroup>(true);
            LookupGroup lookupItems = query.Where<LookupGroup>(x => x.Name == groupName).AsEnumerable().FirstOrDefault();
            lookupItem lookupItem = lookupItems.Items.SingleOrDefault(x => string.Equals(x.Name, itemName, StringComparison.OrdinalIgnoreCase));

            return lookupItem;
        }

        public async Task<LookupGroup> GetItemAsync(string groupName, Guid id)
        {
            var query = _container.GetItemLinqQueryable<LookupGroup>(true);
            LookupGroup lookupItems = query.Where<LookupGroup>(x => x.Name == groupName).AsEnumerable().FirstOrDefault();
            LookupGroup lookupItem = lookupItems.Items.SingleOrDefault(x => string.Equals(x.Id, id.ToString(), StringComparison.OrdinalIgnoreCase));

            return lookupItem;
        }

        public async Task<IEnumerable<LookupGroup>> GetItemsAsync()
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c");

            List<LookupGroup> result = new List<LookupGroup>();
            using (FeedIterator<LookupGroup> feedIterator = _container.GetItemQueryIterator<LookupGroup>(query))
            {
                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<LookupGroup> response = await feedIterator.ReadNextAsync();
                    foreach (var item in response)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }

        public async Task<LookupGroup> CreateItemAsync(LookupGroup lookupGroup)
        {
            var results = await _container.CreateItemAsync<LookupGroup>(lookupGroup, new PartitionKey(lookupGroup.Name));
            return results;
        }
    }
}
