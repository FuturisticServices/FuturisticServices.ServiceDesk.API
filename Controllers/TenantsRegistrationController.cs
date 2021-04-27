using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services;
using TangledServices.ServicePortal.API.Services.System;
using TangledServices.ServicePortal.API.Common;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/tenants")]
    public class TenantsRegistrationController : BaseController
    {
        private readonly ICustomerService _systemTenantsService;
        private readonly ITenantRegistrationService _tenantRegistrationService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        #region Constructors
        public TenantsRegistrationController(ITenantRegistrationService tenantRegistrationService,
                                ICustomerService systemTenantsService,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _systemTenantsService = systemTenantsService;
            _tenantRegistrationService = tenantRegistrationService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region Public methods
        /// <summary>
        /// Creates a [TangledServices.{moniker}.ServicePortal] database.
        /// Inside the database, creates 'LookupItems', 'Subscriptions' and 'Users' containers.
        /// Populates each container with items copied from the system database.
        /// </summary>
        /// <param name="moniker">Unique tenant identifier</param>
        /// <returns>HttpStatus 401 ~ Unauthorized</returns>
        /// <returns>HttpStatus 200 ~ Success</returns>
        /// <returns>HttpStatus 400 ~ Bad request</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] TenantRegistrationModel model)
        {
            try
            {
                await _tenantRegistrationService.Register(model);

                response = new ApiResponse(HttpStatusCode.Created, string.Format("Tenant with moniker '{0}' created successfully.", model.Moniker), null);
                return Ok(new { response });
            }
            catch (CustomerNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, string.Format("Tenant with moniker '{0}' not found in system DB.", model.Moniker), exception, null);
                return BadRequest(new { response });
            }
            catch (Exception exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, "Tenant setup failed.", exception, null);
                return BadRequest(new { response });
            }
        }
        #endregion Public methods
    }
}