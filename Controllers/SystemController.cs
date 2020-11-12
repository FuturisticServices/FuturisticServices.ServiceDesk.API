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
    [Route("api/system")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly ISystemService _systemService;
        private readonly ISystemLookupItemsService _systemLookupItemsService;
        private readonly ISystemSubscriptionsService _systemSubscriptionsService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemController(ISystemService systemService,
                                ISystemLookupItemsService systemLookupItemsService,
                                ISystemSubscriptionsService systemSubscriptionsService,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _systemService = systemService;
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
                //dynamic response = new ExpandoObject();
                var response = new ExpandoObject() as IDictionary<string, Object>;

                DatabaseResponse databaseResponse = await _systemService.CreateDatabase();
                response.Add("database", new { statusCode = databaseResponse.StatusCode, value = databaseResponse.StatusCode == HttpStatusCode.Created ? string.Format("Database '{0}' created.", databaseResponse.Resource.Id) : string.Format("Database '{0}' already exists.", databaseResponse.Resource.Id) });

                if (databaseResponse.StatusCode == HttpStatusCode.Created || databaseResponse.StatusCode == HttpStatusCode.OK)
                {
                    //  Create containers.
                    var containersToCreate = _configuration.GetSection("Reset:Containers.Create").Get<List<ResetContainer>>();

                    foreach (ResetContainer container in containersToCreate)
                    {
                        ContainerResponse containerResponseLookupGroups = await _systemService.CreateContainer(databaseResponse.Database, container.Name, container.PartitionKey);
                        response.Add("container" + container.Name.ToPascalCase(), new { statusCode = containerResponseLookupGroups.StatusCode, value = containerResponseLookupGroups.StatusCode == HttpStatusCode.Created ? string.Format("Container '{0}' created.", container.Name) : string.Format("Container '{0}' already exists.", container.Name) });

                        // Persist lookup items.
                        if (container.Items != null)
                        {
                            foreach (LookupGroup lookupGroupSource in container.Items)
                            {
                                //  Map source lookupGroup to target lookupGroup with a new ID (GUID).
                                LookupGroup lookupGroupTarget = new LookupGroup { Id = lookupGroupSource.Id, Group = lookupGroupSource.Group, LookupName = lookupGroupSource.LookupName, Label = lookupGroupSource.Label, Items = lookupGroupSource.Items };

                                //  Persist item.
                                lookupGroupTarget = await _systemLookupItemsService.CreateItemAsync(lookupGroupTarget);
                            }
                        }

                        // Persist subscriptions.
                        if (container.Subscriptions != null)
                        {
                            foreach (Subscription subscriptionSource in container.Subscriptions)
                            {
                                LookupItem renewalTimeframe = subscriptionSource.RenewalTimeframe == null ? null : await _systemLookupItemsService.GetItemAsync("Subscription Renewal Timeframes", Guid.Parse(subscriptionSource.RenewalTimeframe.Id));

                                //  Map source subscription to target subscription.
                                Subscription subscriptionTarget = new Subscription { Id = subscriptionSource.Id, PartitionKey = subscriptionSource.PartitionKey, Name = subscriptionSource.Name, Description = subscriptionSource.Description, Price = subscriptionSource.Price, PromotionCode = subscriptionSource.PromotionCode, IsExpired = subscriptionSource.IsExpired, RenewalOccurrence = subscriptionSource.RenewalOccurrence, RenewalTimeframe = renewalTimeframe, Highlights = subscriptionSource.Highlights };

                                //  Persist item.
                                subscriptionTarget = await _systemSubscriptionsService.CreateItemAsync(subscriptionTarget);
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