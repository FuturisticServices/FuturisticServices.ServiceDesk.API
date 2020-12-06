using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

namespace FuturisticServices.ServiceDesk.API.Managers
{
    public class SystemBaseManager
    {
        public string _uri;
        public string _primaryKey;

        public string _databaseName;
        public Database _database;
        public CosmosClient _dbClient;
        public Container _container;

        public SystemBaseManager(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            SetConnectionParameters(configuration, webHostEnvironment);

            if (!string.IsNullOrEmpty(_uri) && !string.IsNullOrEmpty(_primaryKey))
            {

                CosmosClientBuilder clientBuilder = new CosmosClientBuilder(_uri, _primaryKey);
                _dbClient = clientBuilder.WithConnectionModeDirect().Build();
            }
        }

        public SystemBaseManager(string containerName, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            SetConnectionParameters(configuration, webHostEnvironment);

            if (!string.IsNullOrEmpty(_uri) && !string.IsNullOrEmpty(_primaryKey))
            {

                CosmosClientBuilder clientBuilder = new CosmosClientBuilder(_uri, _primaryKey);
                _dbClient = clientBuilder.WithConnectionModeDirect().Build();
                _container = _dbClient.GetContainer(_databaseName, containerName);
                _database = _container.Database;
            }
        }

        public async Task SetDatabaseAsync(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            SetConnectionParameters(configuration, webHostEnvironment);
            _database = _dbClient.GetDatabase(_databaseName);
        }

        public void SetConnectionParameters(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName == "Production")
            {
                _uri = configuration["cosmosDb.Production:URI"];
                _primaryKey = configuration["cosmosDb.Production:PrimaryKey"];
                _databaseName = configuration["cosmosDb.Production:SystemDatabaseName"];
            }
            else
            {
                _uri = configuration["cosmosDb.Localhost:URI"];
                _primaryKey = configuration["cosmosDb.Localhost:PrimaryKey"];
                _databaseName = configuration["cosmosDb.Localhost:SystemDatabaseName"];
            }
        }
    }
}
