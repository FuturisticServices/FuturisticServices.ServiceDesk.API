using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using AutoMapper;

using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/{moniker}")]
    public class TenantController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public TenantController(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
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