﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Controllers
{
    /// <summary>
    /// Performs methods for the Service Desk setup process.
    /// </summary>
    [Route("api/{moniker}/token")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ISystemTenantManager _systemTenantsService;

        /// <summary>
        /// Service Desk constructor.
        /// </summary>
        /// <param name="systemTenantService"></param>
        public TokenController(ISystemTenantManager systemTenantService)
        {
            _systemTenantsService = systemTenantService;
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
                SystemTenant tenant = await _systemTenantsService.GetItemAsync(moniker);

                if (tenant.PointOfContact.EmailAddress.Address.Equals(pocEmailAddress, StringComparison.InvariantCultureIgnoreCase))
                {
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
