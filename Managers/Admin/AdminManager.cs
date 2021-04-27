using System;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface IAdminManager
    {
        Task<DatabaseResponse> DeleteDatabase();
        Task<DatabaseResponse> CreateDatabase();
        Task<ContainerResponse> CreateContainer(string name, string partitionKeyName);
        Task<List<string>> GetContainers(List<string> containersToOmit);
    }

    public class AdminManager : AdminBaseManager, IAdminManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminManager(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(null, httpContextAccessor, configuration, webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<DatabaseResponse> DeleteDatabase()
        {
            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(_uri, _primaryKey);
            _dbClient = clientBuilder.WithConnectionModeDirect().Build();
            _database = _dbClient.GetDatabase(_databaseName);
            DatabaseResponse response = await _database.DeleteAsync();

            return response;
        }

        public async Task<DatabaseResponse> CreateDatabase()
        {
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

        public async Task<ContainerResponse> CreateContainer(string name, string partitionKeyPath)
        {
            ContainerProperties containerProperties = new ContainerProperties()
            {
                Id = name,
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

        public async Task<List<string>> GetContainers(List<string> lookupGroupsToClone)
        {
            List<string> containers = new List<string>();

            Database database = _dbClient.GetDatabase(_databaseName);
            FeedIterator<ContainerProperties> iterator = database.GetContainerQueryIterator<ContainerProperties>();
            FeedResponse<ContainerProperties> tenantContainers = await iterator.ReadNextAsync().ConfigureAwait(false);

            foreach (var tenantContainer in tenantContainers) {
                if (lookupGroupsToClone.Contains(tenantContainer.Id)) {
                    containers.Add(tenantContainer.Id);
                }
            }

            return containers;
        }
    }
}