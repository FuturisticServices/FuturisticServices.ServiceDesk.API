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

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/system")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly ISystemService _systemService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemController(ISystemService systemService,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _systemService = systemService;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Deletes and creates the [TangledServices.ServicePortal] system database.  ** USE WITH CAUTION **
        /// </summary>
        /// <returns></returns>
        [HttpGet("reset")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Reset()
        {
            try
            {
                var response = await _systemService.Reset();

                return Ok(new { response });
            }
            catch (Exception ex)
            {
                return BadRequest("Error resetting system database/containers. Error: " + ex.Message);
            }
        }
    }
}