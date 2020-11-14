using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

using AutoMapper;

using FuturisticServices.ServiceDesk.API.Entities;
using FuturisticServices.ServiceDesk.API.Services.System;
using FuturisticServices.ServiceDesk.API.Services.Tenants;
using FuturisticServices.ServiceDesk.API.Services.CosmosDb;
using FuturisticServices.ServiceDesk.API.Common;

namespace FuturisticServices.ServiceDesk.API.Controllers
{
    [Route("api/{moniker}/setup")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly ISystemTenantsService _systemTenantsService;
        private readonly ITenantLookupItemsService _tenantLookupItemsService;
        private readonly ISystemLookupItemsService _systemLookupItemsService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SetupController( ICosmosDbService cosmosDbService,
                                ISystemTenantsService systemCustomerService,
                                ITenantLookupItemsService tenantLookupItemsService,
                                ISystemLookupItemsService systemLookupItemsService,
                                IMapper mapper,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _cosmosDbService = cosmosDbService;
            _systemTenantsService = systemCustomerService;
            _tenantLookupItemsService = tenantLookupItemsService;
            _systemLookupItemsService = systemLookupItemsService;
            _mapper = mapper;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Create tenant database and containers.
        /// </summary>
        /// <param name="moniker">Unique tenant ID used when invoking API calls.</param>
        /// <returns></returns>
        [HttpPost("tenant")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Tenant(string moniker)
        {
            try
            {
                //dynamic response = new ExpandoObject();
                var response = new ExpandoObject() as IDictionary<string, Object>;

                Tenant tenant = await _systemTenantsService.GetItemAsync(moniker);
                DatabaseResponse databaseResponse = await _cosmosDbService.CreateDatabase(tenant);
                response.Add("database", new { statusCode = databaseResponse.StatusCode, value = databaseResponse.StatusCode == HttpStatusCode.Created ? string.Format("Database '{0}' created.", databaseResponse.Resource.Id) : string.Format("Database '{0}' already exists.", databaseResponse.Resource.Id) });

                if (databaseResponse.StatusCode == HttpStatusCode.Created || databaseResponse.StatusCode == HttpStatusCode.OK)
                {
                    //  Create containers.
                    var containersToCreate = _configuration.GetSection("Tenant.Setup:Containers.Create").Get<List<SetupContainer>>();

                    foreach (SetupContainer container in containersToCreate)
                    {
                        ContainerResponse containerResponseLookupGroups = await _cosmosDbService.CreateContainer(databaseResponse.Database, container.Name, container.PartitionKey);
                        response.Add("container" + container.Name.ToPascalCase(), new { statusCode = containerResponseLookupGroups.StatusCode, value = containerResponseLookupGroups.StatusCode == HttpStatusCode.Created ? string.Format("Container '{0}' created.", container.Name) : string.Format("Container '{0}' already exists.", container.Name) });
                    }
                
                    //  Get list of lookup groups to ommit.
                    var lookupGroupsToOmit = _configuration.GetSection("Tenant.Setup:LookupGroups.Omit").GetChildren().Select(x => x.Value).ToList();

                    // Populate lookup items from system database.
                    List<LookupGroup> lookupGroups = (await _systemLookupItemsService.GetItemsAsync()).ToList();
                    foreach (LookupGroup lookupGroupSource in lookupGroups)
                    {
                        //  Skip over groups to ommit.
                        if (lookupGroupsToOmit.Contains(lookupGroupSource.Name)) continue;

                        //  Overwrite existing ID with new GUID.
                        lookupGroupSource.Items.ToList().ForEach(x => x.Id = Guid.NewGuid().ToString());

                        //  Map source lookupGroup to target lookupGroup with a new ID (GUID).
                        LookupGroup lookupGroupTarget = new LookupGroup { Id = Guid.NewGuid().ToString(), Name = lookupGroupSource.Name, Label = lookupGroupSource.Label, Items = lookupGroupSource.Items };
                        
                        //  Persist item.
                        lookupGroupTarget = await _tenantLookupItemsService.CreateItemAsync(lookupGroupTarget);
                    }

                    response.Add("status", this.StatusCode(StatusCodes.Status200OK, "Service Desk setup completed successfully."));
                    return Ok(new { response });
                }

                response.Add("status", this.StatusCode(StatusCodes.Status400BadRequest, "Service Desk setup NOT completed successfully."));
                return BadRequest(new { response });
            }
            catch (Exception ex)
            {
                return BadRequest("Error setting up tenant. Error: " + ex.Message);
            }
        }
    }
}