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
    [Route("api/{moniker}/admin/users")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AdminUsersController : BaseController
    {
        #region Members
        private readonly IAdminUsersService _adminUserService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region IoC Containers
        public AdminUsersController(IAdminUsersService adminUserService,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _adminUserService = adminUserService;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion IoC Containers

        #region Public methods
        /// <summary>
        /// Authenticates a "Basic Auth" header (authorization) username/password against existing customer admin users.
        /// </summary>
        /// <returns>A JWT token.</returns>
        [HttpGet("login")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(string moniker, [FromHeader(Name = "Authorization")] string basicAuthHeader)
        {
            try
            {
                var model = await _adminUserService.AuthenticateAsync(basicAuthHeader);

                if (model != null)
                {
                    AdminAuthenticateUser user = new AdminAuthenticateUser(model);
                    var jwtToken = await _adminUserService.GenerateJwtToken(user);

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
        /// Retrieves and existing customer admin user.
        /// </summary>
        /// <param name="id">Unique GUID identifier.</param>
        /// <returns>AdminUserModel object.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = Enums.Role.SystemRoot + "," + Enums.Role.AdminRoot)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string moniker, string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid isGuid)) throw new GuidNotValidException();

                var adminUser = await _adminUserService.GetItem(new Guid(id));
                var model = new AdminUserModel(adminUser);

                responseModels.Add("AdminUser", model);
                response = new ApiResponse(HttpStatusCode.OK, "Admin user found.", responseModels);
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
        /// Create a new existing system user.
        /// </summary>
        /// <param name="model">SystemUserModel object.</param>
        [HttpPost]
        //[Authorize(Roles = Enums.Role.Root)]
        //[Authorize(Roles = Enums.Role.????)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create(AdminAuthenticateUserModel model)
        {
            try
            {
                await _adminUserService.CreateItem(model);

                response = new ApiResponse(HttpStatusCode.OK, "Admin user created successfully.", null);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Admin user create failed. Error: " + exception.Message);
            }
        }

        /// <summary>
        /// Updates an existing customer admin user.
        /// </summary>
        /// <param name="model">SystemUserModel object.</param>
        [HttpPost("{id}")]
        [Authorize(Roles = Enums.Role.SystemRoot + "," + Enums.Role.AdminRoot)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(AdminUserModel model)
        {
            try
            {
                await _adminUserService.UpdateItem(model);

                response = new ApiResponse(HttpStatusCode.OK, "Admin user updated successfully.", null);
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
        [Authorize(Roles = Enums.Role.SystemRoot + "," + Enums.Role.AdminRoot)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetUsername(AdminUserResetUsernameModel model)
        {
            try
            {
                await _adminUserService.ResetUsername(model);

                response = new ApiResponse(HttpStatusCode.OK, string.Format("Admin user with username '{0}' updated successfully.", model.ResetUsername), null);
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
                response = new ApiResponse(HttpStatusCode.Conflict, string.Format("Username '{0}' already exists in the admin database.", model.ResetUsername), null);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                return BadRequest("Admin user update failed. Error: " + exception.Message);
            }
        }

        /// <summary>
        /// Resets an existing admin user's password.
        /// </summary>
        /// <param name="model">SystemUserResetPasswordModel object</param>
        [HttpPost("reset/password")]
        [Authorize(Roles = Enums.Role.SystemRoot + "," + Enums.Role.AdminRoot)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResetPassword(AdminUserResetPasswordModel model)
        {
            try
            {
                await _adminUserService.ResetPassword(model);

                response = new ApiResponse(HttpStatusCode.OK, "Admin user password reset successfully.", null);
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
                return BadRequest("Admin user update failed. Error: " + exception.Message);
            }
        }

        /// <summary>
        /// Deletes an existing admin user.
        /// </summary>
        /// <param name="id">Unique GUID identifier.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = Enums.Role.SystemRoot + "," + Enums.Role.AdminRoot)]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid isGuid)) throw new GuidNotValidException();

                await _adminUserService.DeleteItem(new Guid(id));

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
