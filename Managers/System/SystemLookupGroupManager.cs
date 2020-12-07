using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using FuturisticServices.ServiceDesk.API.Common;
using FuturisticServices.ServiceDesk.API.Entities;

namespace FuturisticServices.ServiceDesk.API.Managers
{
    public interface ISystemLookupGroupManager
    {
        Task<LookupItem> GetItemAsync(string lookupName, Guid id);
        Task<LookupGroup> GetItemAsync(string groupName);
        Task<LookupItem> GetItemAsync(string groupName, string name);
        Task<IEnumerable<LookupGroup>> GetItemsAsync();
        Task<LookupGroup> CreateItemAsync(LookupGroup lookupGroup);
    }

    public class SystemLookupGroupManager : SystemBaseManager, ISystemLookupGroupManager
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public SystemLookupGroupManager(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("LookupItems", configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<LookupItem> GetItemAsync(string lookupName, Guid id)
        {
            LookupGroup lookupGroup = await GetItemAsync(lookupName);
            var result = lookupGroup.Items.SingleOrDefault(x => x.Id == id.ToString());
            
            return result;
        }

        public async Task<LookupGroup> GetItemAsync(string groupName)
        {
            Enums.LookupGroups lookupGroup;

            groupName = groupName.ToPascalCase();
            if (Enum.TryParse<Enums.LookupGroups>(groupName, true, out lookupGroup))
            {
                var lookupName = lookupGroup.GetDescription().ToCamelCase();

                QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.lookupName = @lookupName")
                .WithParameter("@lookupName", lookupName);

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

        public async Task<LookupItem> GetItemAsync(string groupName, string itemName)
        {
            var query = _container.GetItemLinqQueryable<LookupGroup>(true);
            LookupGroup lookupgroup = query.Where<LookupGroup>(x => x.Group == groupName).AsEnumerable().FirstOrDefault();
            LookupItem lookupItem = lookupgroup.Items.SingleOrDefault(x => x.Name == itemName);

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
            var results = await _container.CreateItemAsync<LookupGroup>(lookupGroup, new PartitionKey(lookupGroup.Group));
            return results;
        }
    }
}
