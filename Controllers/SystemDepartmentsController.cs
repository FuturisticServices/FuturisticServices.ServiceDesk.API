using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/system/departments")]
    [ApiController]
    public class SystemDepartmentsController : BaseController
    {
        private readonly ISystemDepartmentsService _systemDepartmentsService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemDepartmentsController(ISystemDepartmentsService systemDepartmentsService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _systemDepartmentsService = systemDepartmentsService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(bool flattenHierarchy = false, bool includeDeletedItems = false)
        {
            try
            {
                var model = await _systemDepartmentsService.GetItems(flattenHierarchy, includeDeletedItems);

                responseModels.Add("systemDepartments", model);
                response = new ApiResponse(HttpStatusCode.OK, "System departments retrieved successfully", responseModels);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, "System departments NOT retrieved successfully", exception);
                return BadRequest(new { response });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string id, bool includeSubDepartments = true, bool includeDeletedItems = false)
        {
            try
            {
                var model = await _systemDepartmentsService.GetItem(id, includeSubDepartments, includeDeletedItems);

                responseModels.Add("systemDepartment", model);
                response = new ApiResponse(HttpStatusCode.OK, string.Format("System department '{0}' retrieved successfully.", model.Name), responseModels);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, "System departments NOT retrieved successfully", exception);
                return BadRequest(new { response });
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(SystemDepartmentModel model)
        {
            try
            {
                model = await _systemDepartmentsService.UpdateItem(model);

                responseModels.Add("systemDepartment", model);
                response = new ApiResponse(HttpStatusCode.OK, string.Format("System department '{0}' updated successfully.", model.Name), responseModels);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, string.Format("Error updating system department '{0}'.", model.Name), exception);
                return BadRequest(new { response });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _systemDepartmentsService.Delete(id);

                response = new ApiResponse(HttpStatusCode.OK, string.Format("System department with ID '{0}' deleted successfully.", id), responseModels);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, string.Format("Error deleting system department '{0}'.", id), exception);
                return BadRequest(new { response });
            }
        }
    }
}