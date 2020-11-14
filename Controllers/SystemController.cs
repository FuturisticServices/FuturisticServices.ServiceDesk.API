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
using FuturisticServices.ServiceDesk.API.Models;
using FuturisticServices.ServiceDesk.API.Services.System;
using FuturisticServices.ServiceDesk.API.Common;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FuturisticServices.ServiceDesk.API.Controllers
{
    [Route("api/system")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly ISystemService _systemService;
        private readonly ISystemLookupGroupsService _systemLookupGroupsService;
        private readonly ISystemLookupItemsService _systemLookupItemsService;
        private readonly ISystemSubscriptionsService _systemSubscriptionsService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemController(ISystemService systemService,
                                ISystemLookupGroupsService systemLookupGroupsService,
                                ISystemLookupItemsService systemLookupItemsService,
                                ISystemSubscriptionsService systemSubscriptionsService,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _systemService = systemService;
            _systemLookupGroupsService = systemLookupGroupsService;
            _systemLookupItemsService = systemLookupItemsService;
            _systemSubscriptionsService = systemSubscriptionsService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("reset")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Reset()
        {
            try
            {
                var response = new ExpandoObject() as IDictionary<string, Object>;

                DatabaseResponse databaseResponse = await _systemService.CreateDatabase();
                response.Add("database", new { statusCode = databaseResponse.StatusCode, value = databaseResponse.StatusCode == HttpStatusCode.Created ? string.Format("Database '{0}' created.", databaseResponse.Resource.Id) : string.Format("Database '{0}' already exists.", databaseResponse.Resource.Id) });

                if (databaseResponse.StatusCode == HttpStatusCode.Created || databaseResponse.StatusCode == HttpStatusCode.OK)
                {
                    //  Create containers.
                    var containers = _configuration.GetSection("Reset:Containers.Create").Get<List<ResetContainerModel>>();

                    foreach (ResetContainerModel container in containers)
                    {
                        ContainerResponse containerResponse = await _systemService.CreateContainer(databaseResponse.Database, container.Name, container.PartitionKey);
                        response.Add(string.Format("container: {0}", container.Name.ToPascalCase()), new { statusCode = containerResponse.StatusCode, value = containerResponse.StatusCode == HttpStatusCode.Created ? string.Format("Container '{0}' created.", container.Name) : string.Format("Container '{0}' already exists.", container.Name) });

                        // Persist lookup items.
                        if (container.Groups != null)
                        {
                            foreach (LookupGroup group in container.Groups)
                            {
                                //  Persist item.
                                await _systemLookupGroupsService.CreateItemAsync(group);

                                //  Add status to response.
                                response.Add(string.Format("lookup group: {0}", group.Name), new { statusCode = "created", value = string.Format("Group '{0}' created.", group.Name), items = group.Items.ToArray() });
                            }
                        }

                        // Persist subscriptions.
                        if (container.Subscriptions != null)
                        {
                            foreach (Subscription subscription in container.Subscriptions)
                            {
                                //  If a renewal timeframe exists, retrieve it. Otherwise, make it null.
                                subscription.RenewalTimeframe = subscription.RenewalTimeframe == null ? null : await _systemLookupItemsService.GetItemAsync("Subscription Renewal Timeframes", Guid.Parse(subscription.RenewalTimeframe.Id));
                                
                                //  Persist item.
                                await _systemSubscriptionsService.CreateItemAsync(subscription);

                                //  Add status to response.
                                response.Add(string.Format("subscription: {0}", subscription.PartitionKey), new { statusCode = "created", value = string.Format("Subscription '{0}' created.", subscription.PartitionKey) });
                            }
                        }
                    }

                    response.Add("status", this.StatusCode(StatusCodes.Status200OK, "System Service Desk reset completed successfully."));
                    return Ok(new { response });
                }

                response.Add("status", this.StatusCode(StatusCodes.Status400BadRequest, "System Service Desk reset NOT completed successfully."));
                return BadRequest(new { response });
            }
            catch (Exception ex)
            {
                return BadRequest("Error resetting system database/containers. Error: " + ex.Message);
            }
        }
    }
}