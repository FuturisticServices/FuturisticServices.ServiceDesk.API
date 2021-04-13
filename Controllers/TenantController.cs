using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/{moniker}")]
    public class TenantController : BasePortalController
    {
        private readonly IConfiguration _configuration;

        public TenantController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Tenant(string moniker)
        {
            return BadRequest("Not Authorized.");
        }
    }
}