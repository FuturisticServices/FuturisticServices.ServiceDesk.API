using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Services;
using TangledServices.ServicePortal.API.Common;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/tenants/setup")]
    public class TenantSetupController : BasePortalController
    {
        private readonly ISystemTenantsService _systemTenantsService;
        private readonly ITenantSetupService _tenantSetupService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        #region Constructors
        public TenantSetupController(ITenantSetupService tenantSetupService,
                                ISystemTenantsService systemTenantsService,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _systemTenantsService = systemTenantsService;
            _tenantSetupService = tenantSetupService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region Public methods
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Tenant(string moniker)
        {
            return BadRequest("Not Authorized.");
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
        [HttpPost("{moniker}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Setup(string moniker)
        {
            try
            {
                if (await _systemTenantsService.NotExists(moniker)) throw new SystemTenantDoesNotExistException(moniker);

                await _tenantSetupService.Setup(moniker);

                response = new ApiResponse(HttpStatusCode.Created, string.Format("Tenant with moniker '{0}' created successfully.", moniker));
                return Ok(new { response });
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