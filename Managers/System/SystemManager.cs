﻿using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using FuturisticServices.ServiceDesk.API.Common;
using FuturisticServices.ServiceDesk.API.Entities;

namespace FuturisticServices.ServiceDesk.API.Managers
{
    public interface ISystemManager
    {
        Task<DatabaseResponse> DeleteDatabase();
        Task<DatabaseResponse> CreateDatabase();
        Task<ContainerResponse> CreateContainer(Database database, string containerName, string partitionKeyName);
        Task<List<string>> GetContainers(List<string> containersToOmit);
    }

    public class SystemManager : SystemBaseManager, ISystemManager
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public SystemManager(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Subscriptions", configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<DatabaseResponse> DeleteDatabase()
        {
            _databaseName = _webHostEnvironment.EnvironmentName == "Production" ? _configuration["cosmosDb.Production:SystemDatabaseName"] : _configuration["cosmosDb.Localhost:SystemDatabaseName"];

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(_uri, _primaryKey);
            CosmosClient client = clientBuilder.WithConnectionModeDirect().Build();
            Database database = client.GetDatabase(_databaseName);
            DatabaseResponse response = await database.DeleteAsync();

            return response;
        }

        public async Task<DatabaseResponse> CreateDatabase()
        {
            _databaseName = _webHostEnvironment.EnvironmentName == "Production" ? _configuration["cosmosDb.Production:SystemDatabaseName"] : _configuration["cosmosDb.Localhost:SystemDatabaseName"];

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(_uri, _primaryKey);
            CosmosClient client = clientBuilder.WithConnectionModeDirect().Build();
            DatabaseResponse response = await client.CreateDatabaseIfNotExistsAsync(_databaseName);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                response = await DeleteDatabase();
                response = await client.CreateDatabaseAsync(_databaseName);
            }

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

        public async Task<List<string>> GetContainers(List<string> lookupGroupsToClone)
        {
            List<string> containers = new List<string>();

            Database database = _dbClient.GetDatabase(_databaseName);
            FeedIterator<ContainerProperties> iterator = database.GetContainerQueryIterator<ContainerProperties>();
            FeedResponse<ContainerProperties> systemContainers = await iterator.ReadNextAsync().ConfigureAwait(false);

            foreach (var systemContainer in systemContainers) {
                if (lookupGroupsToClone.Contains(systemContainer.Id)) {
                    containers.Add(systemContainer.Id);
                }
            }

            return containers;
        }
    }
}