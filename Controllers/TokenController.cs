using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using FuturisticServices.ServiceDesk.API.Entities;
using FuturisticServices.ServiceDesk.API.Managers;

namespace FuturisticServices.ServiceDesk.API.Controllers
{
    /// <summary>
    /// Performs methods for the Service Desk setup process.
    /// </summary>
    [Route("api/{moniker}/token")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ISystemTenantsManager _systemTenantsService;

        /// <summary>
        /// Service Desk constructor.
        /// </summary>
        /// <param name="systemTenantsService"></param>
        public TokenController(ISystemTenantsManager systemTenantsService)
        {
            _systemTenantsService = systemTenantsService;
        }

        /// <summary>
        /// Determines if a setup token (GUID) is authentic and associated to the proper tenant.
        /// </summary>
        /// <param name="moniker">Unique ID assigned to each tenant used making API requests.</param>
        /// <param name="token">The token (GUID) to validate.</param>
        /// <returns></returns>
        [HttpGet("validate")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Validate(string moniker, [FromQuery][Required] string token)
        {
            try
            {
                bool tokenIsValid = await _systemTenantsService.ValidateToken(moniker, token);

                if (tokenIsValid)
                {
                    ObjectResult statusCode = StatusCode(StatusCodes.Status200OK, "Setup token is authentic.");
                    var response = new { token, tokenIsValid, statusCode };
                    return Ok(new { response });
                }
                else
                {
                    ObjectResult statusCode = StatusCode(StatusCodes.Status400BadRequest, "Setup token is NOT authentic.");
                    var response = new { token, tokenIsValid, statusCode };
                    return BadRequest(new { response });
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error validating setup token. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Validates the moniker and pocEmailAddress is associated to the proper tenant.
        /// If valid, generates a new setup token. Token is emailed to the provided PoC email address.
        /// Otherwise, HTTP-BadRequest.
        /// </summary>
        /// <param name="moniker"></param>
        /// <param name="pocEmailAddress"></param>
        /// <returns></returns>
        [HttpGet("new")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> New(string moniker, [FromQuery][Required] string pocEmailAddress)
        {
            try
            {
                Tenant tenant = await _systemTenantsService.GetItemAsync(moniker);

                if (tenant.PointOfContact.EmailAddress.Address.Equals(pocEmailAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    tenant.SetupToken = Guid.NewGuid().ToString();
                    tenant = await _systemTenantsService.ReplaceItemAsync(tenant);

                    ObjectResult statusCode = StatusCode(StatusCodes.Status200OK, "New token issued.");
                    var response = new { tenant.SetupToken, statusCode };

                    return Ok(new { response });
                }
                else
                {
                    ObjectResult statusCode = StatusCode(StatusCodes.Status400BadRequest, "Token NOT issued. PoC email address does not match.");
                    var response = new { statusCode };

                    return BadRequest(new { response });
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error generating new setup token. Error: " + ex.Message);
            }
        }
    }
}
