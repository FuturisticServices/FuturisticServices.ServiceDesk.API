using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SystemTenantsController(ISystemTenantsService systemTenantService, IMapper mapper, IConfiguration configuration)
        {
            _systemTenantService = systemTenantService;
            _mapper = mapper;
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
                response = new ApiResponse(HttpStatusCode.OK, string.Format("Tenant with moniker '{0}' found.", moniker), null, systemTenant);
                return Ok(new { response });
            }
            catch (SystemTenantDoesNotExistException exception)
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
        public async Task<IActionResult> Create([FromBody] SystemTenantModel model)
        {
            try
            {
                SystemTenant systemTenant = await _systemTenantService.Create(model);
                response = new ApiResponse(HttpStatusCode.OK, string.Format("Tenant with moniker '{0}' created in system DB.", model.Moniker));
                return Ok(new { response });
            }
            catch (SubscriptionNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.NotFound, string.Format("Subscription with ID '{0}' not found in system DB.", model.SubscriptionId));
                return BadRequest(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Error setting up tenant service desk. Error: " + exception.Message);
            }
        }
    }
}