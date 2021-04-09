using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Common;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ISystemTenantsManager
    {
        Task<SystemTenant> CreateItemAsync(SystemTenant systemTenant);
        Task<SystemTenant> DeleteItemAsync(SystemTenant systemTenant);
        Task<IEnumerable<SystemTenant>> GetItemsAsync();
        Task<SystemTenant> GetItemAsync(string moniker);
        Task<SystemTenant> ReplaceItemAsync(SystemTenant systemTenant);

        SystemTenant GetItem(string moniker);
    }

    public class SystemTenantsManager : SystemBaseManager, ISystemTenantsManager
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        public SystemTenantsManager(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Tenants", configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Persists a new tenant object to the container.
        /// </summary>
        /// <param name="tenant">Tenant entity</param>
        /// <returns></returns>
        public async Task<SystemTenant> CreateItemAsync(SystemTenant systemTenant)
        {
            var results = await _container.CreateItemAsync<SystemTenant>(systemTenant, new PartitionKey(systemTenant.Company.Name));
            return results;
        }

        public async Task<SystemTenant> DeleteItemAsync(SystemTenant systemTenant)
        {
            var results = await _container.DeleteItemAsync<SystemTenant>(systemTenant.Id, new PartitionKey(systemTenant.Company.Name));
            return results;
        }

        public async Task<IEnumerable<SystemTenant>> GetItemsAsync()
        {
            var query = _container.GetItemQueryIterator<SystemTenant>(new QueryDefinition("SELECT * FROM c"));

            List<SystemTenant> results = new List<SystemTenant>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.AsEnumerable();
        }

        public async Task<SystemTenant> GetItemAsync(string moniker)
        {
            var query = _container.GetItemLinqQueryable<SystemTenant>();
            var iterator = query.Where(x=>x.Moniker.ToUpper() == moniker.ToUpper()).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<SystemTenant> ReplaceItemAsync(SystemTenant systemTenant)
        {
            SystemTenant customerTarget = await GetItemAsync(systemTenant.Moniker);
            customerTarget = await _container.ReplaceItemAsync<SystemTenant>(customerTarget, customerTarget.Id, new PartitionKey(customerTarget.Company.Name));
            return customerTarget;
        }

        #region Non-async methods
        public SystemTenant GetItem(string moniker)
        {
            var query = _container.GetItemLinqQueryable<SystemTenant>(true);
            SystemTenant tenant = query.Where<SystemTenant>(x => x.Moniker.ToUpper() == moniker.Trim().ToUpper()).AsEnumerable().FirstOrDefault();

            return tenant;
        }
        #endregion Non-async methods
    }
}
