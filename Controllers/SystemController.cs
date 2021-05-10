using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Services;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/system")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SystemController : BaseController
    {
        private readonly ISystemService _systemService;
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemController(ISystemService systemService,
                                ISystemLookupItemService systemLookupItemService,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _systemService = systemService;
            _systemLookupItemService = systemLookupItemService;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Deletes (if exists) and creates the [TangledServices.ServicePortal] system database.  ** USE WITH CAUTION - NOT TURNING BACK! **
        /// Utilizes json data from configurationFiles/system-reset.json.
        /// Utilizes hardcoded GUIDs so item IDs remain the same each time a reset is performed.
        /// </summary>
        /// <returns></returns>
        [HttpPost("reset")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Reset()
        {
            try
            {
                await _systemService.Reset();

                var response = new ApiResponse(HttpStatusCode.OK, "System DB reset successfully.", null);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                var response = new ApiResponse(HttpStatusCode.BadRequest, "System DB reset was unsuccessful.", exception, null);
                return BadRequest(new { response });
            }
        }
    }
}