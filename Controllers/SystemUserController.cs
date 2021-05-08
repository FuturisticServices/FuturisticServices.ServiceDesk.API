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
        #region Members
        private readonly ISystemUsersService _systemUserService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region IoC Containers
        public SystemUserController(ISystemUsersService systemUserService,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _systemUserService = systemUserService;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion IoC Containers

        #region Public methods
        /// <summary>
        /// Authenticates a "Basic Auth" header (authorization) username/password against existing system users.
        /// </summary>
        /// <returns>A JWT token.</returns>
        [HttpGet("login")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromHeader(Name = "Authorization")] string basicAuthHeader)
        {
            try
            {
                var model = await _systemUserService.AuthenticateAsync(basicAuthHeader);

                if (model != null)
                {
                    SystemAuthenticateUser user = new SystemAuthenticateUser(model);
                    var jwtToken = await _systemUserService.GenerateJwtToken(user);

                    responseModels.Add("token", jwtToken);
                    response = new ApiResponse(HttpStatusCode.OK, string.Format("User '{0}' authenticated successfully.", user.Username), responseModels);
                    return Ok(new { response });
                }

                response = new ApiResponse(HttpStatusCode.Unauthorized, "Login failed.", null, null);
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

        /// <summary>
        /// Retrieves and existing system user.
        /// </summary>
        /// <param name="id">Unique GUID identifier.</param>
        /// <returns>SystemUserModel object.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = Enums.Role.Root)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid isGuid)) throw new GuidNotValidException();

                var systemUser = await _systemUserService.GetItem(new Guid(id));
                var model = new SystemUserModel(systemUser);

                responseModels.Add("SystemUser", model);
                response = new ApiResponse(HttpStatusCode.OK, "System user found.", responseModels);
                return Ok(new { response });
            }
            catch (GuidNotValidException exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, null, exception, null);
                return Ok(new { response });
            }
            catch (UserNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.NotFound, null, exception, null);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, null, exception, null);
                return Ok(new { response });
            }
        }

        /// <summary>
        /// Updates an existing system user.
        /// </summary>
        /// <param name="model">SystemUserModel object.</param>
        [HttpPost]
        //[Authorize(Roles = Enums.Role.Root)]
        //[Authorize(Roles = Enums.Role.????)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(SystemUserModel model)
        {
            try
            {
                await _systemUserService.UpdateItem(model);

                response = new ApiResponse(HttpStatusCode.OK, "System user updated successfully.", null);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("System user update failed. Error: " + exception.Message);
            }
        }

        /// <summary>
        /// Resets an existing system user's username.
        /// </summary>
        /// <param name="model">SystemUserResetUsernameModel object.</param>
        [HttpPost("reset/username")]
        //[Authorize(Roles = Enums.Role.Root)]
        //[Authorize(Roles = Enums.Role.????)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetUsername(SystemUserResetUsernameModel model)
        {
            try
            {
                await _systemUserService.ResetUsername(model);

                response = new ApiResponse(HttpStatusCode.OK, string.Format("System user with username '{0}' updated successfully.", model.ResetUsername), null);
                return Ok(new { response });
            }
            catch (UserNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.NotFound, exception.Message, null);
                return Ok(new { response });
            }
            catch (PasswordsDoNotMatchException exception)
            {
                response = new ApiResponse(HttpStatusCode.Conflict, exception.Message, null);
                return Ok(new { response });
            }
            catch (UsernameAlreadyExistsException exception)
            {
                response = new ApiResponse(HttpStatusCode.Conflict, string.Format("Username '{0}' already exists in the system database.", model.ResetUsername), null);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("System user update failed. Error: " + exception.Message);
            }
        }

        /// <summary>
        /// Resets an existing system user's password.
        /// </summary>
        /// <param name="model">SystemUserResetPasswordModel object</param>
        [HttpPost("reset/password")]
        //[Authorize(Roles = Enums.Role.Root)]
        //[Authorize(Roles = Enums.Role.????)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetPassword(SystemUserResetPasswordModel model)
        {
            try
            {
                await _systemUserService.ResetPassword(model);

                response = new ApiResponse(HttpStatusCode.OK, "System user password reset successfully.", null);
                return Ok(new { response });
            }
            catch (UserNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.NotFound, exception.Message, null);
                return Ok(new { response });
            }
            catch (PasswordsDoNotMatchException exception)
            {
                response = new ApiResponse(HttpStatusCode.Conflict, exception.Message, null);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("System user update failed. Error: " + exception.Message);
            }
        }

        /// <summary>
        /// Deletes an existing system user.
        /// </summary>
        /// <param name="id">Unique GUID identifier.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = Enums.Role.Root), Authorize(Roles = Enums.Role.Root)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid isGuid)) throw new GuidNotValidException();

                await _systemUserService.DeleteItem(new Guid(id));

                response = new ApiResponse(HttpStatusCode.OK, "System user deleted successully.", null, null);
                return Ok(new { response });
            }
            catch (GuidNotValidException exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, null, exception, null);
                return Ok(new { response });
            }
            catch (UserNotFoundException exception)
            {
                response = new ApiResponse(HttpStatusCode.NotFound, null, exception, null);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                response = new ApiResponse(HttpStatusCode.BadRequest, null, exception, null);
                return Ok(new { response });
            }
        }
        #endregion Public methods
    }
}