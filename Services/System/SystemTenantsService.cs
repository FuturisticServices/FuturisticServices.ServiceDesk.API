using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

using FuturisticServices.ServiceDesk.API.Entities;
using FuturisticServices.ServiceDesk.API.Common;

namespace FuturisticServices.ServiceDesk.API.Services.System
{
    public interface ISystemTenantsService
    {
        Task<Tenant> CreateItemAsync(Tenant tenant);
        Task<Tenant[]> GetItemsAsync();
        Tenant GetItem(string moniker);
        Task<Tenant> GetItemAsync(string moniker);
        Task<Tenant> ReplaceItemAsync(Tenant tenant);
        Task<bool> ValidateToken(string moniker, string setupToken);
        Task<string> NewToken(string moniker, string pocEmailAddress);
    }

    public class SystemTenantsService : SystemBaseService, ISystemTenantsService
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        public SystemTenantsService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Tenants", configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Persists a new tenant object to the container.
        /// </summary>
        /// <param name="tenant">Tenant entity</param>
        /// <returns></returns>
        public async Task<Tenant> CreateItemAsync(Tenant tenant)
        {
            var results = await _container.CreateItemAsync<Tenant>(tenant, new PartitionKey(tenant.Company.Name));
            return results;
        }

        public async Task<Tenant[]> GetItemsAsync()
        {
            var query = _container.GetItemQueryIterator<Tenant>(new QueryDefinition("SELECT * FROM c"));

            List<Tenant> results = new List<Tenant>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results.ToArray();
        }

        public Tenant GetItem(string moniker)
        {
            var query = _container.GetItemLinqQueryable<Tenant>(true);
            Tenant tenant = query.Where<Tenant>(x => x.Moniker == moniker.Trim().ToUpper()).AsEnumerable().FirstOrDefault();

            return tenant;
        }

        public async Task<Tenant> GetItemAsync(string moniker)
        {
            var query = _container.GetItemQueryIterator<Tenant>(new QueryDefinition("SELECT * FROM c WHERE LOWER(c.moniker) = '" + moniker.ToLower() + "'"));

            Tenant tenant = new Tenant();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                tenant = response.SingleOrDefault();
            }

            return tenant;
        }

        public async Task<Tenant> ReplaceItemAsync(Tenant tenant)
        {
            Tenant customerTarget = await GetItemAsync(tenant.Moniker);
            customerTarget.SetupToken = tenant.SetupToken;
            customerTarget = await _container.ReplaceItemAsync<Tenant>(customerTarget, customerTarget.Id, new PartitionKey(customerTarget.Company.Name));
            return customerTarget;
        }

        /// <summary>
        /// Authenticates a setup token is associated to an existing tenant.
        /// </summary>
        /// <param name="moniker">Unique tenant ID used to invoke APIs.</param>
        /// <param name="setupToken">GUID representing the setup token.</param>
        /// <returns>True if the token is authentic. False otherwise.</returns>
        public async Task<bool> ValidateToken(string moniker, string setupToken)
        {
            Tenant tenant = await GetItemAsync(moniker);
            bool setupTokenIsValid = tenant != null && tenant.SetupToken.Equals(setupToken, StringComparison.InvariantCultureIgnoreCase) ? true : false;

            return setupTokenIsValid;
        }

        public async Task<string> NewToken(string moniker, string pocEmailAddress)
        {
            string newToken = null;

            Tenant tenant = await GetItemAsync(moniker);

            if (tenant != null && tenant.PointOfContact.EmailAddress.Address.Equals(pocEmailAddress, StringComparison.InvariantCultureIgnoreCase))
            {
                tenant.SetupToken = Guid.NewGuid().ToString();
                tenant = await ReplaceItemAsync(tenant);
            }

            return newToken;
        }

        //public async Task<DatabaseConnection> GetDatabaseInformation(Tenant tenant, List<LookupGroup> systemLookupItems)
        //{
        //    List<LookupItem> datababasePlatforms = systemLookupItems.Where(x => x.LookupName == Enums.LookupGroups.DatabasePlatforms.GetDescription().ToCamelCase()).SelectMany(x => x.Items).ToList();
        //    LookupItem lookupItemAzureCosmosDb = datababasePlatforms.SingleOrDefault(x => x.LookupName == Enums.DatabasePlatforms.azureCosmosDb.GetDescription().ToCamelCase());
        //    string databaseName = string.Format("Futuristic.{0}.ServiceDesk", tenant.Moniker);

        //    DatabaseConnection databaseConnection = new DatabaseConnection(lookupItemAzureCosmosDb, databaseName, );

        //    if (tenant != null && tenant.PointOfContact.EmailAddress.Address.Equals(pocEmailAddress, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        tenant.SetupToken = Guid.NewGuid().ToString();
        //        tenant = await ReplaceItemAsync(tenant);
        //    }

        //    return newToken;
        //}
    }
}
