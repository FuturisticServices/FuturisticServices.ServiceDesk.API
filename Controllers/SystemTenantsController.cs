using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/system/tenants")]
    public class SystemTenantsController : BasePortalController
    {
        private readonly ISystemTenantsService _systemTenantService;
        private readonly ISystemSubscriptionService _systemSubscriptionService;
        private readonly IConfiguration _configuration;

        public SystemTenantsController(ISystemTenantsService systemTenantService, ISystemSubscriptionService systemSubscriptionService, IConfiguration configuration)
        {
            _systemSubscriptionService = systemSubscriptionService;
            _systemTenantService = systemTenantService;
            _configuration = configuration;
        }

        /// <summary>
        /// Sets up a tenant in the [tangled.services] database Tenant container.
        /// Tenant details come from the 'tenants.json' config file with matching moniker.
        /// </summary>
        /// <param name="moniker"></param>
        /// <returns>HttpStatus 200 ~ Success</returns>
        /// <returns>HttpStatus 400 ~ Bad request</returns>
        [HttpGet("{moniker}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string moniker)
        {
            try
            {
                moniker = moniker.ToUpper().Trim();
                var tenant = await _systemTenantService.Get(moniker);

                SystemTenant systemTenant = await _systemTenantService.Get(moniker);
                SystemTenantModel model = new SystemTenantModel(systemTenant);
                response = new ApiResponse(HttpStatusCode.OK, string.Format("Tenant with moniker '{0}' found.", moniker), null, new List<Object>() { systemTenant });
                return Ok(new { response });
            }
            catch (SystemTenantDoesNotExistException)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, string.Format("Tenant with moniker '{0}' not found in system DB.", moniker));
                return BadRequest(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Error setting up tenant service desk. Error: " + exception.Message);
            }
        }
        
        /// <summary>
        /// Creates a [TangledServices.{moniker}.ServicePortal] database.
        /// Inside the database, creates a 'LookupItems', 'Subscriptions' and 'Users' containers.
        /// Populates each container with appropriate items.
        /// </summary>
        /// <param name="moniker">ID unique to each Tenant used in all API requests.</param>
        /// <returns>HttpStatus 401 ~ Unauthorized</returns>
        /// <returns>HttpStatus 200 ~ Success</returns>
        /// <returns>HttpStatus 400 ~ Bad request</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] SystemTenantCreateModel model)
        {
            try
            {
                SystemTenantModel systemTenantModel = await _systemTenantService.Create(model);
                response = new ApiResponse(HttpStatusCode.OK, string.Format("Tenant with moniker '{0}' created in system DB.", model.Moniker), null, new List<Object>() { systemTenantModel });
                return Ok(new { response });
            }
            catch (SubscriptionNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.NotFound, exception.Message, model.Subscription.Id);
                return BadRequest(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Error setting up tenant service desk. Error: " + exception.Message);
            }
        }
    }
}