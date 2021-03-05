using System;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using AutoMapper;

using TangledServices.ServiceDesk.API.Entities;
using TangledServices.ServiceDesk.API.Managers;
using Microsoft.AspNetCore.Http;

namespace TangledServices.ServiceDesk.API.Controllers
{
    [Route("api/{moniker}/[controller]")]
    public class UserController : Controller
    {
        private readonly ISystemTenantManager _systemTenantsService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private Tenant _systemTenant;

        public UserController(ISystemTenantManager customerService, IMapper mapper, IConfiguration configuration)
        {
            _systemTenantsService = customerService;
            _mapper = mapper;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenicates a username/password. If successful, returns a JWT token. Otherwise, login failed.
        /// </summary>
        /// <param name="moniker">Unique tenant ID used in URL routing.</param>
        /// <returns></returns>
        [HttpGet("authenticate")]
        public IActionResult authenticate(string moniker)
        {
            dynamic response = new ExpandoObject();
            
            try
            {
                var _systemTenant = _systemTenantsService.GetItem(moniker);

                var header = Request.Headers["Authorization"];
                if (header.ToString().StartsWith("Basic"))
                {
                    var userCredentials = header.ToString().Substring("Basic ".Length).Trim();
                    var usernameAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(userCredentials)).Split(':');
                    var username = usernameAndPassword[0];
                    var password = usernameAndPassword[1];

                    //  Authenticate user against tenant container.

                    if (username == "system.admin" && password == "password")
                    {
                        string jwtSecretKey = _configuration.GetSection("jwt:secretKey").Value;

                        var claimsData = new[] { new Claim(ClaimTypes.Name, username) };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                        var jwtSecurityToken = new JwtSecurityToken(
                            issuer: "futuristic.service",
                            audience: "futuristic.service",
                            expires: DateTime.Now.AddDays(365),
                            claims: claimsData,
                            signingCredentials: credentials
                        );
                        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                        response.token = token;
                        return Ok(new { response });
                    }
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