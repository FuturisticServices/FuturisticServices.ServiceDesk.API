using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServiceDesk.API.Common;
using TangledServices.ServiceDesk.API.Entities;
using TangledServices.ServiceDesk.API.Managers;
using TangledServices.ServiceDesk.API.Models;
using TangledServices.ServiceDesk.API.Services;

namespace TangledServices.ServiceDesk.API.Services
{
    public interface ICosmosDbService {
        Task<DatabaseResponse> CreateDatabase(Tenant tenant);
        Task<ContainerResponse> CreateContainer(Database database, string containerName, string partitionKeyName);
        Container GetContainer(Database database, string containerName);
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
        public async Task<DatabaseResponse> CreateDatabase(Tenant tenant)
        {
            DatabaseResponse response = await _cosmosDbManager.CreateDatabaseAsync(tenant);
            return response;
        }

        public async Task<ContainerResponse> CreateContainer(Database database, string containerName, string partitionKeyName)
        {
            ContainerResponse response = await _cosmosDbManager.CreateContainerAsync(database, containerName, partitionKeyName);
            return response;
        }

        public Container GetContainer(Database database, string containerName)
        {
            Container response = _cosmosDbManager.GetContainer(database, containerName);
            return response;
        }
        #endregion Public methods
    }
}