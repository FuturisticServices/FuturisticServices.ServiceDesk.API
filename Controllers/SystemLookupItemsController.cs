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
        /// Retrieves all System LookupItem groups.
        /// </summary>
        /// <returns>List of SystemLookupItemModel objects.</returns>
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
        /// Retrieves an existing System LookupItem group.
        /// </summary>
        /// <param name="key">Either a GUID (ID) or name of the System LookupItem group to retrieve.</param>
        /// <returns>SystemLookupItemModel object.</returns>
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
        /// Creates a new System LookupItem group.
        /// </summary>
        /// <param name="model">SystemLookupItemModel object.</param>
        /// <returns>SystemLookupItemModel object.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(SystemLookupItemModel model)
        {
            try
            {
                model = await _systemLookupItemService.CreateItem(model);

                responseModels.Add("LookupItem", model);
                response = new ApiResponse(HttpStatusCode.OK, string.Format("LookupItem '{0}' created.", model.Name), responseModels);
                return Ok(new { response });
            }
            catch (SystemLookupItemAlreadyExistsException exception)
            {
                response = new ApiResponse(HttpStatusCode.Conflict, string.Format("System LookupItem group '{0}' already exists.", model.Name), exception, null);
                return Unauthorized(new { response });
            }
            catch (Exception exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, string.Format("Error creating System LookupItem group '{0}'.", model.Name), exception, null);
                return BadRequest(new { response });
            }
        }

        /// <summary>
        /// Updates an existing System LookupItem group.
        /// </summary>
        /// <param name="model">SystemLookupItemModel object.</param>
        /// <returns>401 ~ Not authorized or invalid JWT token.</returns>
        /// <returns>200 ~ OK</returns>
        /// <returns>400 ~ Bad request</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(SystemLookupItemModel model)
        {
            try
            {
                model = await _systemLookupItemService.Update(model);

                responseModels.Add("systemLookupItem", model);
                response = new ApiResponse(HttpStatusCode.OK, string.Format("System LookupItem '{0}' updated successfully.", model.Name), responseModels);
                return Ok(new { response });
            }
            catch (SystemLookupItemNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.NotFound, string.Format("System LookupItem '{0}' NOT found.", model.Name), exception, null);
                return Unauthorized(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Error updating LookupItem in system database. Error: " + exception.Message);
            }
        }

        [HttpDelete("{groupKey}/{itemKey}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string groupKey, string itemKey)
        {
            var groupKeyIsGuid = Guid.TryParse(groupKey, out var groupId);
            var itemKeyIsGuid = Guid.TryParse(itemKey, out var itemId);

            try
            {
                var model = await _systemLookupItemService.DeleteItem(groupKey, itemKey);

                var responseMessage = string.Empty;
                if (groupKeyIsGuid && itemKeyIsGuid)
                {
                    responseMessage = string.Format("System LookupItem group ID '{0}', item ID '{1}' deleted successfully.", groupId, itemId);
                }
                else if (!groupKeyIsGuid && itemKeyIsGuid)
                {
                    responseMessage = string.Format("System LookupItem group '{0}', item ID '{1}' deleted successfully.", groupKey, itemId);
                }
                else if (groupKeyIsGuid && !itemKeyIsGuid)
                {
                    responseMessage = string.Format("System LookupItem group ID '{0}', item '{1}' deleted successfully.", groupId, itemKey);
                }
                else if (!groupKeyIsGuid && !itemKeyIsGuid)
                {
                    responseMessage = string.Format("System LookupItem group '{0}', item '{1}' deleted successfully.", groupKey, itemKey);
                }

                responseModels.Add("Lookup Item", model);
                response = new ApiResponse(HttpStatusCode.OK, responseMessage, null, responseModels);
                return Ok(new { response });
            }
            catch (SystemLookupItemNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.Conflict, exception.Message, null);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Error setting up tenant service desk. Error: " + exception.Message);
            }
        }
    }
}