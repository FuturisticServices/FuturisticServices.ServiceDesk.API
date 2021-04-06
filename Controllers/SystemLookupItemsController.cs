using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using AutoMapper;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/system/lookupitems")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SystemLookupItemsController : ControllerBase
    {
        private readonly ISystemLookupItemService _systemService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly dynamic _response = new ExpandoObject();

        public SystemLookupItemsController(ISystemLookupItemService systemService, IMapper mapper, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _systemService = systemService;
            _mapper = mapper;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Retrieves all items in the [LookupItems] container from the [TangledServices.ServicePortal] database.
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

                var groups = await _systemService.GetItems();

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
        /// Retrieves all items in the [LookupItems] container with a group = {groupName} from the [TangledServices.ServicePortal] database.
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
                    
                var group = await _systemService.GetItem(groupName);

                if (group != null)
                {
                    response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("{0} '{1}' retrieved.", group.Items.Count(), groupName));
                    response.data = group;
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

        /// <summary>
        /// Creates a new item in the [LookupItems] container in the [TangledServices.ServicePortal] database.
        /// </summary>
        /// <param name="group">Object {LookupGroup} to create.</param>
        /// <returns></returns>
        /// /// <returns>401 ~ Not authorized or invalid JWT token.</returns>
        /// <returns>200 ~ OK</returns>
        /// <returns>400 ~ Bad request</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] LookupGroupPostModel model)
        {
            try
            {
                var existingGroup = await _systemService.CreateItem(model);

                if (existingGroup != null)
                {
                    _response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("LookupItem group '{0}' created.", model.Group));
                    _response.model = model;
                    return Ok(new { _response });
                }

                _response.status = this.StatusCode(StatusCodes.Status400BadRequest, string.Format("LookupItem group '{0}' already exists.", model.Group));
                return BadRequest(new { _response });
            }
            catch (Exception ex)
            {
                _response.group = null;
                _response.status = this.StatusCode(StatusCodes.Status400BadRequest, string.Format("Error creating LookupItem '{0}'. Error: {1}", model.Group, ex.Message));
                return BadRequest(new { _response });
            }
        }

        /// <summary>
        /// Update one or more group items in the LookupItems container.
        /// </summary>
        /// <param name="groupName">Name of the group associated to common items.</param>
        /// <param name="lookupItemsToUpdate">Array of <LookupItem> containing item properties to update.</LookupItem></param>
        /// <returns>401 ~ Not authorized or invalid JWT token.</returns>
        /// <returns>200 ~ OK</returns>
        /// <returns>400 ~ Bad request</returns>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromBody] LookupGroupEntity group)
        {
            try
            {
                group = await _systemService.UpdateGroup(group);

                if (group != null)
                {
                    _response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("LookupItem group '{0}' updated.", group.Group));
                    _response.data = group;
                    return Ok(new { _response });
                }

                _response.status = this.StatusCode(StatusCodes.Status400BadRequest, string.Format("LookupItem group '{0}' does not exist.", group.Group));
                return BadRequest(new { _response });
            }
            catch (Exception ex)
            {
                _response.group = null;
                _response.status = this.StatusCode(StatusCodes.Status400BadRequest, string.Format("Error updating LookupItem '{0}'. Error: {1}", group.Group, ex.Message));
                return BadRequest(new { _response });
            }
        }
    }
}