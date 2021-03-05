using System.Dynamic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using AutoMapper;

using TangledServices.ServiceDesk.API.Managers;

namespace TangledServices.ServiceDesk.API.Controllers
{
    [Route("api/system/{moniker}")]
    public class SystemTenantController : Controller
    {
        private readonly ISystemTenantManager _systemTenantService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SystemTenantController(ISystemTenantManager systemTenantService, IMapper mapper, IConfiguration configuration)
        {
            _systemTenantService = systemTenantService;
            _mapper = mapper;
            _configuration = configuration;
        }

        /// <summary>
        /// Sets up a tenant in the [futuristic.services] database Tenant container.
        /// Tenant details come from the 'tenants.json' config file with matching moniker.
        /// </summary>
        /// <param name="moniker"></param>
        /// <returns>HttpStatus 200 ~ Success</returns>
        /// <returns>HttpStatus 400 ~ Bad request</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Tenant(string moniker)
        {
            dynamic response = new ExpandoObject();

            var tenant = await _systemTenantService.GetItemAsync(moniker);

            if (tenant != null)
            {
                response.tenant = tenant;
                response.status = this.StatusCode(StatusCodes.Status200OK, "Tenant found.");
                return Ok(new { response });
            }

            response.tenant = tenant;
            response.status = this.StatusCode(StatusCodes.Status400BadRequest, "Tenant not found.");
            return BadRequest(new { response });
        }
    }
}