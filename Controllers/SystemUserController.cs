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

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/system/users")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SystemUserController : BaseController
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
        [HttpPost("login")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromHeader(Name = "Authorization")] string basicAuthHeader)
        {
            dynamic response = new ExpandoObject();

            try
            {
                var model = await _systemUserService.AuthenticateAsync(basicAuthHeader);

                if (model != null)
                {
                    SystemUser user = new SystemUser(model);
                    var jwtToken = await _systemUserService.GenerateJwtToken(user);

                    responseModels.Add("jwtToken", jwtToken);
                    //responseModels.Add("User", model);
                    response = new ApiResponse(HttpStatusCode.OK, string.Format("User '{0}' authenticated successfully.", user.Username), responseModels);
                    return Ok(new { response });
                }

                response.status = this.StatusCode(StatusCodes.Status401Unauthorized, "Login failed.");
                return Unauthorized(new { response });
            }
            catch (UserNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.Unauthorized, "Login failed.", exception, null);
                return Unauthorized(new { response });
            }
            catch (LoginFailedException exception)
            {
                response = new ApiResponse(HttpStatusCode.Unauthorized, "Login failed", exception, null);
                return Unauthorized(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Login failed. Error: " + exception.Message);
            }
        }
    }
}