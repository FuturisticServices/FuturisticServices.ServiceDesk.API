using System;
using System.Dynamic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using AutoMapper;

using FuturisticServices.ServiceDesk.API.Managers;

namespace FuturisticServices.ServiceDesk.API.Controllers
{
    [Route("api/system/lookupitems/{groupName}")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SystemLookupItemsController : ControllerBase
    {
        private readonly ISystemLookupItemManager _systemService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemLookupItemsController(ISystemLookupItemManager systemService, IMapper mapper, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _systemService = systemService;
            _mapper = mapper;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Establishes a new tenant in the FuturisticServices.ServiceDesk --> Tenants container.
        /// </summary>
        /// <param name="groupName">The 'systemGroup' value related to a group of items.</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string groupName)
        {
            try
            {
                dynamic response = new ExpandoObject();

                var items = await _systemService.GetItemAsync(groupName);

                if (items != null)
                {
                    response.items = items;
                    response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("Items for '{0}' retrieved successfully.", groupName));
                    return Ok(new { response });
                }

                response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("No Items found for '{0}'.", groupName));
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                dynamic response = new ExpandoObject();
                response.items = null;
                response.status = this.StatusCode(StatusCodes.Status400BadRequest, string.Format("Error retrieving items for '{0}'.", groupName));

                return BadRequest(new { response });
            }
        }
    }
}