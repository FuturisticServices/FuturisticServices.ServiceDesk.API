using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using FuturisticServices.ServiceDesk.API.Common;
using FuturisticServices.ServiceDesk.API.Entities;
using Microsoft.Azure.Cosmos.Linq;
using FuturisticServices.ServiceDesk.API.Extensions;

namespace FuturisticServices.ServiceDesk.API.Managers
{
    public interface ISystemLookupItemManager
    {
        Task<LookupItem> GetItemAsync(string groupName, Guid id);
        Task<LookupGroup> GetItemAsync(string groupName);
        Task<LookupItem> GetItemAsync(string groupName, string name);
        Task<IEnumerable<LookupGroup>> GetItemsAsync();
        Task<LookupItem> CreateItemAsync(LookupItem lookupItem);
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

        public async Task<LookupGroup> GetItemAsync(string groupName)
        {
            groupName = groupName.ToTitleCase();

            var query = _container.GetItemLinqQueryable<LookupGroup>();
            var iterator = query.Where(x => x.Name == groupName).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<LookupItem> GetItemAsync(string groupName, Guid id)
        {
            var lookupGroup = await GetItemAsync(groupName);
            var result = lookupGroup.Items.SingleOrDefault(x => x.Id == id.ToString());

            return result;
        }

        public async Task<LookupItem> GetItemAsync(string groupName, string itemName)
        {
            var lookupGroup = await GetItemAsync(groupName);
            var result = lookupGroup.Items.SingleOrDefault(x => x.Name == itemName);

            return result;
        }

        public async Task<IEnumerable<LookupGroup>> GetItemsAsync()
        {
            List<LookupGroup> result = new List<LookupGroup>();

            var query = _container.GetItemLinqQueryable<LookupGroup>();
            var iterator = query.ToFeedIterator();
            FeedResponse<LookupGroup> response = await iterator.ReadNextAsync();
            foreach (var item in response) result.Add(item);
            return result;

            //  SQL async call.
            //QueryDefinition query = new QueryDefinition("SELECT * FROM c");

            //List<LookupGroup> result = new List<LookupGroup>();
            //using (FeedIterator<LookupGroup> feedIterator = _container.GetItemQueryIterator<LookupGroup>(query))
            //{
            //    while (feedIterator.HasMoreResults)
            //    {
            //        FeedResponse<LookupGroup> response = await feedIterator.ReadNextAsync();
            //        foreach (var item in response)
            //        {
            //            result.Add(item);
            //        }
            //    }
            //}

            //return result;
        }

        public async Task<LookupItem> CreateItemAsync(LookupItem lookupItem)
        {
            var results = await _container.CreateItemAsync<LookupItem>(lookupItem);
            return results;
        }
    }
}
