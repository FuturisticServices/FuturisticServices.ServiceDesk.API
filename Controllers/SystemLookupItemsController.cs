using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/system/lookupitems")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SystemLookupItemsController : BaseController
    {
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly dynamic _response = new ExpandoObject();

        public SystemLookupItemsController(ISystemLookupItemService systemLookupItemService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _systemLookupItemService = systemLookupItemService;
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var model = await _systemLookupItemService.GetItems();

                responseModels.Add("Lookup Items", model);
                response = new ApiResponse(HttpStatusCode.OK, "System LookupItems found.", null, responseModels);
                return Ok(new { response });
            }
            catch (SystemLookupItemsNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.Conflict, "System LookupItems not found.", null);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Error setting up tenant service desk. Error: " + exception.Message);
            }
        }

        /// <summary>
        /// Retrieves a System LookupItem group by ID or group name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("{key}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string key)
        {
            var isGuid = Guid.TryParse(key, out var id);

            try
            {
                var model = isGuid ? await _systemLookupItemService.GetItem(id) : await _systemLookupItemService.GetItem(key);
                var responseMessage = isGuid ? string.Format("System LookupItem with ID '{0}' found.", id) : string.Format("System LookupItem group '{0}' found.", key);
                responseModels.Add("Lookup Items", model);
                response = new ApiResponse(HttpStatusCode.OK, responseMessage, null, responseModels);
                return Ok(new { response });
            }
            catch (SystemLookupItemNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.Conflict, isGuid ? string.Format("System LookupItem with ID '{0}' not found.", key) : string.Format("System LookupItem group '{0}' not found.", key), null);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Error setting up tenant service desk. Error: " + exception.Message);
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
        public async Task<IActionResult> Post([FromBody] SystemLookupItemModel model)
        {
            try
            {
                var existingGroup = await _systemLookupItemService.CreateItem(model);

                if (existingGroup != null)
                {
                    _response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("LookupItem group '{0}' created.", model.Name));
                    _response.model = model;
                    return Ok(new { _response });
                }

                _response.status = this.StatusCode(StatusCodes.Status400BadRequest, string.Format("LookupItem group '{0}' already exists.", model.Name));
                return BadRequest(new { _response });
            }
            catch (Exception ex)
            {
                _response.group = null;
                _response.status = this.StatusCode(StatusCodes.Status400BadRequest, string.Format("Error creating LookupItem '{0}'. Error: {1}", model.Name, ex.Message));
                return BadRequest(new { _response });
            }
        }

        /// <summary>
        /// Updates an existing SystemLookupItem item.
        /// </summary>
        /// <param name="model">SystemLookupItemModel object.</param>
        /// <returns>401 ~ Not authorized or invalid JWT token.</returns>
        /// <returns>200 ~ OK</returns>
        /// <returns>400 ~ Bad request</returns>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromBody] SystemLookupItemModel model)
        {
            try
            {
                model = await _systemLookupItemService.Update(model);

                responseModels.Add("systemLookupItem", model);
                response = new ApiResponse(HttpStatusCode.OK, string.Format("SystemLookupItem '{0}' found.", model.DisplayAs), responseModels);
                return Ok(new { response });
            }
            catch (SystemLookupItemNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.Unauthorized, "Access denied", exception, null);
                return Unauthorized(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Error updating LookupItem in system database. Error: " + exception.Message);
            }
        }
    }
}