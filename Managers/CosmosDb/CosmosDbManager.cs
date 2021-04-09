using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ICosmosDbManager
    {
        Task<DatabaseResponse> CreateDatabaseAsync(SystemTenant systemTenant);
        Task<ContainerResponse> CreateContainerIfNotExistsAsync(Database database, string containerName, string partitionKeyName);
        Container GetContainer(Database database, string containerName);
        Task<ContainerResponse> DeleteContainer(Database database, string containerName);
    }

    public class CosmosDbManager : CosmosDbBaseManager, ICosmosDbManager
    {
        #region Members
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
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
        #endregion Constructors

        #region Public methods
        /// <summary>
        /// Persists a new tenant object to the container.
        /// </summary>
        /// <param name="tenant">Tenant entity</param>
        /// <returns></returns>
        public async Task<DatabaseResponse> CreateDatabaseAsync(SystemTenant systemTenant)
        {
            _databaseName = string.Format(_webHostEnvironment.EnvironmentName == "Production" ? _configuration["cosmosDb.Production:TenantDatabaseName"] : _configuration["cosmosDb.Localhost:TenantDatabaseName"], systemTenant.Moniker.ToUpper());

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(_uri, _primaryKey);
            CosmosClient client = clientBuilder.WithConnectionModeDirect().Build();
            DatabaseResponse response = await client.CreateDatabaseIfNotExistsAsync(_databaseName);

            //if (response.StatusCode == HttpStatusCode.OK)
            //{
            //    response = await DeleteDatabaseAsync(systemTenant);
            //    response = await client.CreateDatabaseAsync(_databaseName);
            //}

            return response;
        }

        public async Task<ContainerResponse> CreateContainerIfNotExistsAsync(Database database, string containerName, string partitionKeyName)
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

        public Container GetContainer(Database database, string containerName)
        {
            Container response = database.GetContainer(containerName);
            return response;
        }

        public async Task<ContainerResponse> DeleteContainer(Database database, string containerName)
        {
            Container container = GetContainer(database, containerName);
            ContainerResponse response = await container.DeleteContainerAsync();
            return response;
        }
        #endregion Public methods

        #region Private methods
        private async Task<DatabaseResponse> DeleteDatabaseAsync(SystemTenant systemTenant)
        {
            _databaseName = string.Format(_webHostEnvironment.EnvironmentName == "Production" ? _configuration["cosmosDb.Production:TenantDatabaseName"] : _configuration["cosmosDb.Localhost:TenantDatabaseName"], systemTenant.Moniker.ToUpper());

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(_uri, _primaryKey);
            CosmosClient client = clientBuilder.WithConnectionModeDirect().Build();
            Database database = client.GetDatabase(_databaseName);
            DatabaseResponse response = await database.DeleteAsync();

            return response;
        }
        #endregion  Private methods
    }
}
