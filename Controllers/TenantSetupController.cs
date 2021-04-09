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
using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services;
using TangledServices.ServicePortal.API.Common;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/tenants/{moniker}")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TenantSetupController : BasePortalController
    {
        #region Members
        private readonly ISystemTenantsService _systemTenantsService;
        private readonly ITenantSetupService _tenantSetupService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public TenantSetupController(ITenantSetupService tenantSetupService,
                                ISystemTenantsService systemTenantsService,
                                IMapper mapper,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _systemTenantsService = systemTenantsService;
            _tenantSetupService = tenantSetupService;
            _mapper = mapper;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region Public methods
        /// <summary>
        /// Creates a [TangledServices.{moniker}.ServicePortal] database.
        /// Inside the database, creates a 'LookupItems', 'Subscriptions' and 'Users' containers.
        /// Populates each container with appropriate items.
        /// </summary>
        /// <param name="moniker">ID unique to each Tenant used in all API requests.</param>
        /// <returns>HttpStatus 401 ~ Unauthorized</returns>
        /// <returns>HttpStatus 200 ~ Success</returns>
        /// <returns>HttpStatus 400 ~ Bad request</returns>
        [HttpPost("setup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Setup(string moniker)
        {
            try
            {
                if (await _systemTenantsService.NotExists(moniker)) throw new SystemTenantDoesNotExistException(moniker);

                Tenant tenant = await _tenantSetupService.Setup(moniker);
                TenantModels tenantModel = new TenantModels() { Moniker = tenant.Moniker, Company = tenant.Company, SubscriptionId = tenant.SubscriptionId, BillingInformation = tenant.BillingInformation, PointOfContact = tenant.PointOfContact };

                response = new ApiResponse(HttpStatusCode.Created, string.Format("Tenant with moniker '{0}' created successfully.", moniker));
                return Ok( new { response });
            }
            catch (SystemTenantDoesNotExistException exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, string.Format("Tenant with moniker '{0}' not found in system DB.", moniker), exception.Message);
                return BadRequest(new { response });
            }
            catch (Exception exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, "Tenant setup failed.", exception.Message);
                return BadRequest(new { response });
            }
        }
        #endregion Public methods
    }
}