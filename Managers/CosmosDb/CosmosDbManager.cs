using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

using FuturisticServices.ServiceDesk.API.Entities;

namespace FuturisticServices.ServiceDesk.API.Managers
{
    public interface ICosmosDbManager
    {
        Task<DatabaseResponse> DeleteDatabase(Tenant tenant);
        Task<DatabaseResponse> CreateDatabase(Tenant tenant);
        Task<ContainerResponse> CreateContainer(Database database, string containerName, string partitionKeyName);
    }

    public class CosmosDbManager : CosmosDbBaseManager, ICosmosDbManager
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        public CosmosDbManager(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Tenants", configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<DatabaseResponse> DeleteDatabase(Tenant tenant)
        {
            _databaseName = string.Format(_webHostEnvironment.EnvironmentName == "Production" ? _configuration["cosmosDb.Production:TenantDatabaseName"] : _configuration["cosmosDb.Localhost:TenantDatabaseName"], tenant.Moniker.ToUpper());

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(_uri, _primaryKey);
            CosmosClient client = clientBuilder.WithConnectionModeDirect().Build();
            Database database = client.GetDatabase(_databaseName);
            DatabaseResponse response = await database.DeleteAsync();

            return response;
        }

        /// <summary>
        /// Persists a new tenant object to the container.
        /// </summary>
        /// <param name="tenant">Tenant entity</param>
        /// <returns></returns>
        public async Task<DatabaseResponse> CreateDatabase(Tenant tenant)
        {
            _databaseName = string.Format(_webHostEnvironment.EnvironmentName == "Production" ? _configuration["cosmosDb.Production:TenantDatabaseName"] : _configuration["cosmosDb.Localhost:TenantDatabaseName"], tenant.Moniker.ToUpper());

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(_uri, _primaryKey);
            CosmosClient client = clientBuilder.WithConnectionModeDirect().Build();
            DatabaseResponse response = await client.CreateDatabaseIfNotExistsAsync(_databaseName);

            return response;
        }

        public async Task<ContainerResponse> CreateContainer(Database database, string containerName, string partitionKeyName)
        {
            ContainerProperties containerProperties = new ContainerProperties()
            {
                Id = containerName,
                PartitionKeyPath = string.Format("/{0}", partitionKeyName),
                IndexingPolicy = new IndexingPolicy()
                {
                    Automatic = false,
                    IndexingMode = IndexingMode.Lazy,
                }
            };

            ContainerResponse response = await database.CreateContainerIfNotExistsAsync(containerProperties);

            return response;
        }
    }
}
