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
    public interface ICustomersManager
    {
        Task<CustomerEntity> CreateItemAsync(CustomerEntity systemTenant);
        Task<CustomerEntity> UpserItemAsync(CustomerEntity systemTenant);
        Task<CustomerEntity> DeleteItemAsync(CustomerEntity systemTenant);
        Task<IEnumerable<CustomerEntity>> GetItemsAsync();
        Task<CustomerEntity> GetItemAsync(Guid id);
        Task<CustomerEntity> GetItemAsync(string adminMoniker);
        Task<CustomerEntity> GetItemByAdminMonikerAsync(string adminMoniker);
    }

    public class CustomersManager : SystemBaseManager, ICustomersManager
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        public CustomersManager(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Customers", configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Async methods
        /// <summary>
        /// Persists a new tenant object to the container.
        /// </summary>
        /// <param name="tenant">Tenant entity</param>
        /// <returns></returns>
        public async Task<CustomerEntity> CreateItemAsync(CustomerEntity entity)
        {
            var results = await _container.CreateItemAsync<CustomerEntity>(entity, new PartitionKey(entity.LegalEntityName));
            return results;
        }

        public async Task<IEnumerable<CustomerEntity>> GetItemsAsync()
        {
            var query = _container.GetItemQueryIterator<CustomerEntity>(new QueryDefinition("SELECT * FROM c"));

            List<CustomerEntity> results = new List<CustomerEntity>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.AsEnumerable();
        }

        public async Task<CustomerEntity> GetItemAsync(Guid id)
        {
            var query = _container.GetItemLinqQueryable<CustomerEntity>();
            var iterator = query.Where(x => x.Id == id.ToString()).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<CustomerEntity> GetItemAsync(string moniker)
        {
            var query = _container.GetItemLinqQueryable<CustomerEntity>();
            var iterator = query.Where(x=>x.AdminMoniker.ToUpper() == moniker.ToUpper()).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        public async Task<CustomerEntity> GetItemByAdminMonikerAsync(string adminMoniker)
        {
            var query = _container.GetItemLinqQueryable<CustomerEntity>();
            var iterator = query.Where(x => x.AdminMoniker.ToUpper() == adminMoniker.ToUpper()).ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Persists a new tenant object to the container.
        /// </summary>
        /// <param name="tenant">Tenant entity</param>
        /// <returns></returns>
        public async Task<CustomerEntity> UpserItemAsync(CustomerEntity entity)
        {
            var results = await _container.UpsertItemAsync<CustomerEntity>(entity);
            return results;
        }

        public async Task<CustomerEntity> DeleteItemAsync(CustomerEntity entity)
        {
            var results = await _container.DeleteItemAsync<CustomerEntity>(entity.Id, new PartitionKey(entity.Name));
            return results;
        }
        #endregion Async methods

        //#region Non-async methods
        //public CustomerEntity GetItem(string moniker)
        //{
        //    var query = _container.GetItemLinqQueryable<CustomerEntity>(true);
        //    CustomerEntity tenant = query.Where<CustomerEntity>(x => x.Moniker.ToUpper() == moniker.Trim().ToUpper()).AsEnumerable().FirstOrDefault();

        //    return tenant;
        //}
        //#endregion Non-async methods
    }
}
