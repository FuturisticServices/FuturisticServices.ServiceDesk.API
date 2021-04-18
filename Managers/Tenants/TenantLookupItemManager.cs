﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ITenantLookupItemManager
    {
        Task<LookupGroupEntity> GetItemAsync(string groupName);
        Task<LookupItemEntity> GetItemAsync(string groupName, string itemName);
        Task<LookupItemEntity> GetItemAsync(string groupName, Guid id);
        Task<IEnumerable<LookupGroupEntity>> GetItemsAsync();
        Task<LookupGroupEntity> CreateItemAsync(LookupGroupEntity lookupGroup);

}

public class TenantLookupItemManager : TenantBaseManager, ITenantLookupItemManager
    {
        internal ISystemTenantsManager _systemTenantsService;
        internal IHttpContextAccessor _httpContextAccessor;
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public TenantLookupItemManager(ISystemTenantsManager systemTenantsService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("LookupItems", systemTenantsService, httpContextAccessor, configuration, webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<LookupGroupEntity> GetItemAsync(string groupName)
        {
            Enums.LookupGroups lookupGroup;

            groupName = groupName.ToCamelCase();
            if (Enum.TryParse<Enums.LookupGroups>(groupName, true, out lookupGroup))
            {
                groupName = lookupGroup.GetDescription().ToCamelCase();

                QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.lookupName = @groupName")
                .WithParameter("@groupName", groupName);

                LookupGroupEntity result = new LookupGroupEntity();
                using (FeedIterator<LookupGroupEntity> feedIterator = _container.GetItemQueryIterator<LookupGroupEntity>(query))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        FeedResponse<LookupGroupEntity> response = await feedIterator.ReadNextAsync();
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

        public async Task<LookupItemEntity> GetItemAsync(string groupName, string itemName)
        {
            var query = _container.GetItemLinqQueryable<LookupGroupEntity>(true);
            LookupGroupEntity lookupItems = query.Where<LookupGroupEntity>(x => x.Group == groupName).AsEnumerable().FirstOrDefault();
            LookupItemEntity lookupItem = lookupItems.Items.SingleOrDefault(x => string.Equals(x.Name, itemName, StringComparison.OrdinalIgnoreCase));

            return lookupItem;
        }

        public async Task<LookupItemEntity> GetItemAsync(string groupName, Guid id)
        {
            var query = _container.GetItemLinqQueryable<LookupGroupEntity>(true);
            LookupGroupEntity lookupItems = query.Where<LookupGroupEntity>(x => x.Group == groupName).AsEnumerable().FirstOrDefault();
            LookupItemEntity lookupItem = lookupItems.Items.SingleOrDefault(x => string.Equals(x.Id, id.ToString(), StringComparison.OrdinalIgnoreCase));

            return lookupItem;
        }

        public async Task<IEnumerable<LookupGroupEntity>> GetItemsAsync()
        {
            QueryDefinition query = new QueryDefinition("SELECT * FROM c");

            List<LookupGroupEntity> result = new List<LookupGroupEntity>();
            using (FeedIterator<LookupGroupEntity> feedIterator = _container.GetItemQueryIterator<LookupGroupEntity>(query))
            {
                while (feedIterator.HasMoreResults)
                {
                    FeedResponse<LookupGroupEntity> response = await feedIterator.ReadNextAsync();
                    foreach (var item in response)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }

        public async Task<LookupGroupEntity> CreateItemAsync(LookupGroupEntity lookupGroup)
        {
            var results = await _container.CreateItemAsync<LookupGroupEntity>(lookupGroup, new PartitionKey(lookupGroup.Group));
            return results;
        }
    }
}