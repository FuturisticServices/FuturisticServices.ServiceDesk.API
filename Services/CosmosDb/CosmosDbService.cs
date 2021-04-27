using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ICosmosDbService {
        Task<DatabaseResponse> CreateDatabase(CustomerEntity systemTenant);
        Task<ContainerResponse> CreateContainer(Database database, string containerName, string partitionKeyName);
        Task<List<string>> GetContainers(Database database);
        Container GetContainer(Database database, string containerName);
        bool Exists(Database database, string containerName);
    }

    public class CosmosDbService : CosmosDbBaseService, ICosmosDbService
    {
        #region Members
        private readonly ICosmosDbManager _cosmosDbManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public CosmosDbService(ICosmosDbManager cosmosDbManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _cosmosDbManager = cosmosDbManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region Public methods
        public async Task<DatabaseResponse> CreateDatabase(CustomerEntity systemTenant)
        {
            DatabaseResponse response = await _cosmosDbManager.CreateDatabaseAsync(systemTenant);
            return response;
        }

        public async Task<ContainerResponse> CreateContainer(Database database, string containerName, string partitionKeyName)
        {
            ContainerResponse response = await _cosmosDbManager.CreateContainerIfNotExistAsync(database, containerName, partitionKeyName);
            return response;
        }

        public async Task<List<string>> GetContainers(Database database)
        {
            FeedIterator<ContainerProperties> iterator = database.GetContainerQueryIterator<ContainerProperties>();
            FeedResponse<ContainerProperties> containers = await iterator.ReadNextAsync().ConfigureAwait(false);

            return containers.ToList().Select(x => x.Id).ToList();
        }

        public Container GetContainer(Database database, string containerName)
        {
            Container response = _cosmosDbManager.GetContainer(database, containerName);
            return response;
        }

        public bool Exists(Database database, string containerName)
        {
            Container container = _cosmosDbManager.GetContainer(database, containerName);
            return container == null ? false : true;
        }
        #endregion Public methods
    }
}