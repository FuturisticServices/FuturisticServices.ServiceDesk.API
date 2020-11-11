using System;
using System.Dynamic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using AutoMapper;

using ProjectVolume.ServiceDesk.API.Services.System;

namespace ProjectVolume.ServiceDesk.API.Controllers
{
    [Route("api/tenants")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ISystemTenantsService _systemTenantsService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CustomersController(ISystemTenantsService customerService, IMapper mapper, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _systemTenantsService = customerService;
            _mapper = mapper;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Retrieves all tenants.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            try
            {
                dynamic response = new ExpandoObject();

                var tenants = await _systemTenantsService.GetItemsAsync();


                if (tenants != null)
                {
                    response.tenants = tenants;
                    response.status = this.StatusCode(StatusCodes.Status200OK, "Tenants retrieved successfully.");
                    return Ok(new { response });
                }

                response.status = this.StatusCode(StatusCodes.Status400BadRequest, "No tenants found.");
                return BadRequest(new { response });
            }
            catch (Exception ex)
            {
                return BadRequest("Error retrieving tenants.");
            }
        }

        /// <summary>
        /// Retrieves tenants associated with a moniker.
        /// </summary>
        /// <param name="moniker">Moniker associated to a tenant.</param>
        /// <returns></returns>
        [HttpGet, Route("{moniker}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get(string moniker)
        {
            try
            {
                dynamic response = new ExpandoObject();

                var tenant = await _systemTenantsService.GetItemAsync(moniker);

                if (tenant != null)
                {
                    response.tenant = tenant;
                    response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("Tenant '{0}' retrieved successfully.", moniker));
                    return Ok(new { response });
                }

                response.status = this.StatusCode(StatusCodes.Status400BadRequest, string.Format("Tenant '{0}' not found.", moniker));
                return BadRequest(new { response });
            }
            catch (Exception ex)
            {
                return BadRequest("Error retrieving tenants.");
            }
        }
    }
}