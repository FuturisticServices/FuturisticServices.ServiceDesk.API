using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/system/user")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SystemUserController : ControllerBase
    {
        private readonly ISystemUsersService _systemUserService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemUserController(ISystemUsersService systemUserService,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _systemUserService = systemUserService;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Deletes and creates the [tangled.services] system database. 
        /// ** USE WITH CAUTION **
        /// </summary>
        /// <returns></returns>
        [HttpPost("authenticate")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Authenticate([FromHeader(Name = "Authorization")] string basicAuthHeader)
        {
            dynamic response = new ExpandoObject();

            try
            {
                var user = await _systemUserService.AuthenticateAsync(basicAuthHeader);

                if (user != null)
                {
                    response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("User '{0}' authenticated successfully.", user.Username));
                    response.token = await _systemUserService.GenerateJwtToken(user);
                    return Ok(new { response });
                }

                response.status = this.StatusCode(StatusCodes.Status401Unauthorized, "Login failed.");
                return Unauthorized(new { response });
            }
            catch (Exception ex)
            {
                response.status = this.StatusCode(StatusCodes.Status401Unauthorized, "Login failed. Error: " + ex.Message);
                return Unauthorized(new { response });
            }
        }
    }
}