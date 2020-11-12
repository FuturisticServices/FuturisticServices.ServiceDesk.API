using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using FuturisticServices.ServiceDesk.API.Common;
using FuturisticServices.ServiceDesk.API.Entities;

namespace FuturisticServices.ServiceDesk.API.Services.System
{
    public interface ISystemService
    {
        Task<DatabaseResponse> CreateDatabase();
        Task<ContainerResponse> CreateContainer(Database database, string containerName, string partitionKeyName);
    }

    public class SystemService : SystemBaseService, ISystemService
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public SystemService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Subscriptions", configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<DatabaseResponse> CreateDatabase()
        {
            _databaseName = _webHostEnvironment.EnvironmentName == "Production" ? _configuration["cosmosDb.Production:SystemDatabaseName"] : _configuration["cosmosDb.Localhost:SystemDatabaseName"];

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
