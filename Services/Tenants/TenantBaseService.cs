using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Extensions.Configuration;

using FuturisticServices.ServiceDesk.API.Entities;
using FuturisticServices.ServiceDesk.API.Services.System;

namespace FuturisticServices.ServiceDesk.API.Services.Tenants
{
    public class TenantBaseService
    {
        public string _uri;
        public string _primaryKey;

        public Tenant _tenant;
        public string _moniker;
        public string _databaseName;
        public CosmosClient _dbClient;
        public Microsoft.Azure.Cosmos.Container _container;

        public TenantBaseService(string containerName, ISystemTenantsService systemTenantsService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _moniker = httpContextAccessor.HttpContext.Request.RouteValues["moniker"] != null ? httpContextAccessor.HttpContext.Request.RouteValues["moniker"].ToString() : string.Empty;
            _tenant = systemTenantsService.GetItem(_moniker);

            if (_tenant == null) return;  //  No tenant, no entry!

            SetConnectionParameters(configuration, webHostEnvironment);

            if (!string.IsNullOrEmpty(_uri) && !string.IsNullOrEmpty(_primaryKey))
            {
                CosmosClientBuilder clientBuilder = new CosmosClientBuilder(_uri, _primaryKey);
                _dbClient = clientBuilder.WithConnectionModeDirect().Build();
                _container = _dbClient.GetContainer(_databaseName, containerName);
            }
        }

        public void SetConnectionParameters(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName == "Production")
            {
                _uri = configuration["cosmosDb.Production:URI"];
                _primaryKey = configuration["cosmosDb.Production:PrimaryKey"];
                _databaseName = string.Format(configuration["cosmosDb.Production:TenantDatabaseName"], _moniker.ToUpper());
            }
            else
            {
                _uri = configuration["cosmosDb.Localhost:URI"];
                _primaryKey = configuration["cosmosDb.Localhost:PrimaryKey"];
                _databaseName = string.Format(configuration["cosmosDb.Localhost:TenantDatabaseName"], _moniker.ToUpper());
            }
        }
    }
}
