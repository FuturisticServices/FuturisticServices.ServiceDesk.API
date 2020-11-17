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
using FuturisticServices.ServiceDesk.API.Managers;
using FuturisticServices.ServiceDesk.API.Common;

namespace FuturisticServices.ServiceDesk.API.Controllers
{
    [Route("api/{moniker}/setup")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TenantSetupController : ControllerBase
    {
        private readonly ICosmosDbManager _cosmosDbManager;
        private readonly ITenantLookupItemsManager _tenantLookupItemsManager;
        private readonly ITenantUsersManager _tenantUsersManager;
        private readonly ISystemTenantsManager _systemTenantsManager;
        private readonly ISystemLookupItemsManager _systemLookupItemsManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TenantSetupController( ICosmosDbManager cosmosDbManager,
                                ITenantLookupItemsManager tenantLookupItemsManager,
                                ITenantUsersManager tenantUsersManager,
                                ISystemTenantsManager systemCustomerManager,
                                ISystemLookupItemsManager systemLookupItemsManager,
                                IMapper mapper,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _cosmosDbManager = cosmosDbManager;
            _tenantLookupItemsManager = tenantLookupItemsManager;
            _tenantUsersManager = tenantUsersManager;
            _systemTenantsManager = systemCustomerManager;
            _systemLookupItemsManager = systemLookupItemsManager;
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

                Tenant tenant = await _systemTenantsManager.GetItemAsync(moniker);
                DatabaseResponse databaseResponse = await _cosmosDbManager.CreateDatabase(tenant);
                response.Add("database", new { statusCode = databaseResponse.StatusCode, value = databaseResponse.StatusCode == HttpStatusCode.Created ? string.Format("Database '{0}' created.", databaseResponse.Resource.Id) : string.Format("Database '{0}' already exists.", databaseResponse.Resource.Id) });

                if (databaseResponse.StatusCode == HttpStatusCode.Created || databaseResponse.StatusCode == HttpStatusCode.OK)
                {
                    //  Retrieve tenant containers to create.
                    var containers = _configuration.GetSection("Tenant.Setup:Containers").Get<List<SetupContainer>>();

                    foreach (SetupContainer container in containers)
                    {
                        ContainerResponse containerResponse = await _cosmosDbManager.CreateContainer(databaseResponse.Database, container.Name, container.PartitionKey);
                        response.Add(string.Format("container: {0}", container.Name.ToPascalCase()), new { statusCode = containerResponse.StatusCode, value = containerResponse.StatusCode == HttpStatusCode.Created ? string.Format("Container '{0}' created.", container.Name) : string.Format("Container '{0}' already exists.", container.Name) });
                    }
                
                    //  Get list of lookup groups to omit.
                    var lookupGroupsToOmit = _configuration.GetSection("Tenant.Setup:LookupGroups:Omit").GetChildren().Select(x => x.Value).ToList();

                    // Populate lookup items from system database.
                    List<LookupGroup> lookupGroups = (await _systemLookupItemsManager.GetItemsAsync()).ToList();
                    foreach (LookupGroup group in lookupGroups)
                    {
                        //  Skip over groups to ommit.
                        if (lookupGroupsToOmit.Contains(group.Name)) continue;

                        //  Overwrite existing ID with new GUID unique to tenant.
                        group.Items.ToList().ForEach(x => x.Id = Guid.NewGuid().ToString());

                        //  Persist item.
                        await _tenantLookupItemsManager.CreateItemAsync(group);

                        //  Add status to response.
                        response.Add(string.Format("lookup group: {0}", group.Name), new { statusCode = "created", value = string.Format("Group '{0}' created.", group.Name), items = group.Items.ToArray() });
                    }

                    //  Retrieve tenant users to create.
                    var users = _configuration.GetSection("Tenant.Setup:Users").Get<List<Entities.User>>();

                    // Persist tenant users.
                    foreach (Entities.User user in users)
                    {
                        //  Create unique user ID.
                        user.Id = Guid.NewGuid().ToString();

                        //  Persist item.
                        await _tenantUsersManager.CreateItemAsync(user);

                        //  Add status to response.
                        response.Add(string.Format("user: {0} {1}", user.NameFirst, user.NameLast), new { statusCode = "created", value = string.Format("User '{0} {1}' created.", user.NameFirst, user.NameLast) });
                    }

                    response.Add("status", this.StatusCode(StatusCodes.Status200OK, "Tenat service Desk setup completed successfully."));
                    return Ok(new { response });
                }

                response.Add("status", this.StatusCode(StatusCodes.Status400BadRequest, "Tenant service Desk setup NOT completed successfully."));
                return BadRequest(new { response });
            }
            catch (Exception ex)
            {
                return BadRequest("Error setting up tenant service desk. Error: " + ex.Message);
            }
        }
    }
}