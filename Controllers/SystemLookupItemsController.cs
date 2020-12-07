using System;
using System.Dynamic;
using System.Linq;
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
    [Route("api/system/lookupitems")]
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
        /// Retrieves all items associated to the specified group from the [FuturisticServices.ServiceDesk] database 'LookupItems' container.
        /// </summary>
        /// <returns>401 ~ Not authorized or invalid JWT token.</returns>
        /// <returns>200 ~ OK</returns>
        /// <returns>400 ~ Bad request</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            try
            {
                dynamic response = new ExpandoObject();

                var groups = await _systemService.GetItemsAsync();

                if (groups.Any())
                {
                    response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("{0} groups retrieved.", groups.Count()));
                    response.groups = groups;
                    return Ok(new { response });
                }

                response.status = this.StatusCode(StatusCodes.Status200OK, "No groups found.");
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                dynamic response = new ExpandoObject();
                response.items = null;
                response.status = this.StatusCode(StatusCodes.Status400BadRequest, "Error retrieving groups.");

                return BadRequest(new { response });
            }
        }

        /// <summary>
        /// Retrieves all items associated to the specified group from the [FuturisticServices.ServiceDesk] database 'LookupItems' container.
        /// </summary>
        /// <param name="groupName">The name of group to retrieve.</param>
        /// <returns>401 ~ Not authorized or invalid JWT token.</returns>
        /// <returns>200 ~ OK</returns>
        /// <returns>400 ~ Bad request</returns>
        [HttpGet("{groupName}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string groupName)
        {
            try
            {
                dynamic response = new ExpandoObject();
                    
                var group = await _systemService.GetItemAsync(groupName);

                if (group != null)
                {
                    response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("{0} '{1}' retrieved.", group.Items.Count(), groupName));
                    response.items = group.Items;
                    return Ok(new { response });
                }

                response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("No '{0}' found.", groupName));
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                dynamic response = new ExpandoObject();
                response.items = null;
                response.status = this.StatusCode(StatusCodes.Status400BadRequest, string.Format("Error retrieving '{0}'.", groupName));

                return BadRequest(new { response });
            }
        }
    }
}