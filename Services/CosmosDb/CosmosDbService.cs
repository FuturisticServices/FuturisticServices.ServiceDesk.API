using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

using FuturisticServices.ServiceDesk.API.Entities;
using FuturisticServices.ServiceDesk.API.Services;
using System.Runtime.CompilerServices;

namespace FuturisticServices.ServiceDesk.API.Services.CosmosDb
{
    public interface ICosmosDbService
    {
        Task<DatabaseResponse> CreateDatabase(Tenant tenant);
        Task<ContainerResponse> CreateContainer(Database database, string containerName, string partitionKeyName);
    }

    public class CosmosDbService : CosmosDbBaseService, ICosmosDbService
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        public CosmosDbService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Tenants", configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Persists a new tenant object to the container.
        /// </summary>
        /// <param name="tenant">Tenant entity</param>
        /// <returns></returns>
        public async Task<DatabaseResponse> CreateDatabase(Tenant tenant)
        {
            _databaseName = string.Format("Futuristic.{0}.ServiceDesk", tenant.Moniker.ToUpper());

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
