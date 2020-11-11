using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using AutoMapper;

using FuturisticServices.ServiceDesk.API.Services.Tenants;

namespace FuturisticServices.ServiceDesk.API.Controllers
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
        public IActionResult Tenant(string moniker)
        {
            

            return BadRequest("Not Authorized.");
        }
    }
}