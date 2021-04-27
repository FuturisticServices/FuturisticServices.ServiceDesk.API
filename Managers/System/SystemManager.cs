using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ISystemManager
    {
        Task<DatabaseResponse> DeleteDatabase();
        Task<DatabaseResponse> CreateDatabase();
        Task<ContainerResponse> CreateContainer(string containerName, string partitionKeyName);
        Task<ContainerProperties> GetContainerProperties(string name);
        Task<IEnumerable<Container>> GetContainers();
        Container GetContainer(string name);
    }

    public class SystemManager : SystemBaseManager, ISystemManager
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public SystemManager(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<DatabaseResponse> DeleteDatabase()
        {
            _databaseName = _webHostEnvironment.EnvironmentName == "Production" ? _configuration["cosmosDb.Production:SystemDatabaseName"] : _configuration["cosmosDb.Localhost:SystemDatabaseName"];

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(_uri, _primaryKey);
            _dbClient = clientBuilder.WithConnectionModeDirect().Build();
            _database = _dbClient.GetDatabase(_databaseName);
            DatabaseResponse response = await _database.DeleteAsync();

            return response;
        }

        public async Task<DatabaseResponse> CreateDatabase()
        {
            _databaseName = _webHostEnvironment.EnvironmentName == "Production" ? _configuration["cosmosDb.Production:SystemDatabaseName"] : _configuration["cosmosDb.Localhost:SystemDatabaseName"];

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(_uri, _primaryKey);
            _dbClient = clientBuilder.WithConnectionModeDirect().Build();
            DatabaseResponse response = await _dbClient.CreateDatabaseIfNotExistsAsync(_databaseName);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                response = await DeleteDatabase();
                response = await _dbClient.CreateDatabaseAsync(_databaseName);
            }

            _database = response.Database;

            return response;
        }

        public async Task<ContainerResponse> CreateContainer(string containerName, string partitionKeyPath)
        {
            ContainerProperties containerProperties = new ContainerProperties()
            {
                Id = containerName,
                PartitionKeyPath = partitionKeyPath,
                IndexingPolicy = new IndexingPolicy()
                {
                    Automatic = false,
                    IndexingMode = IndexingMode.Lazy,
                }
            };

            ContainerResponse response = await _database.CreateContainerIfNotExistsAsync(containerProperties);

            return response;
        }

        public async Task<ContainerProperties> GetContainerProperties(string name)
        {
            Database database = _dbClient.GetDatabase(_databaseName);
            FeedIterator<ContainerProperties> iterator = database.GetContainerQueryIterator<ContainerProperties>();
            FeedResponse<ContainerProperties> dbContainers = await iterator.ReadNextAsync().ConfigureAwait(false);

            ContainerProperties containerProperties = new ContainerProperties();
            foreach (var dbContainer in dbContainers)
            {
                if (string.Compare(dbContainer.Id, name, true) != 0) continue;

                containerProperties = dbContainer;
                break;
            }

            return containerProperties;
        }

        public async Task<IEnumerable<Container>> GetContainers()
        {
            Database database = _dbClient.GetDatabase(_databaseName);
            FeedIterator<ContainerProperties> iterator = database.GetContainerQueryIterator<ContainerProperties>();
            FeedResponse<ContainerProperties> dbContainers = await iterator.ReadNextAsync().ConfigureAwait(false);

            List<Container> containers = new List<Container>();
            foreach (var containerProperties in dbContainers)
            {
                Container container = GetContainer(containerProperties.Id);
                containers.Add(container);
            }

            return containers;
        }

        public Container GetContainer(string name)
        {
            Database database = _dbClient.GetDatabase(_databaseName);
            Container response = database.GetContainer(name);
            return response;
        }
    }
}