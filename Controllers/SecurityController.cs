using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services;

namespace TangledServices.ServicePortal.API.Controllers
{
    [Route("api/security")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SecurityController : BaseController
    {
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SecurityController(IHashingService hashingService,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _hashingService = hashingService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("encrypt")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Encrypt(string text)
        {
            try
            {
                string encryptedText = _hashingService.EncryptString(text);

                responseModels.Add("Encrypted text", encryptedText);
                var response = new ApiResponse(HttpStatusCode.OK, "Cypher text decrypted successfully.", responseModels);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                var response = new ApiResponse(HttpStatusCode.BadRequest, "Cypher decrypt NOT successful.", exception, null);
                return BadRequest(new { response });
            }
        }

        /// <summary>
        /// Deletes (if exists) and creates the [TangledServices.ServicePortal] system database.  ** USE WITH CAUTION - NOT TURNING BACK! **
        /// Utilizes json data from configurationFiles/system-reset.json.
        /// Utilizes hardcoded GUIDs so item IDs remain the same each time a reset is performed.
        /// </summary>
        /// <returns></returns>
        [HttpGet("decrypt")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Decrypt(string cypher)
        {
            try
            {
                string decryptedText = _hashingService.DecryptString(cypher);

                responseModels.Add("Decrypted text", decryptedText);
                var response = new ApiResponse(HttpStatusCode.OK, "Cypher text decrypted successfully.", responseModels);
                return Ok(new { response });
            }
            catch (Exception exception)
            {
                var response = new ApiResponse(HttpStatusCode.BadRequest, "Cypher decrypt NOT successful.", exception, null);
                return BadRequest(new { response });
            }
        }
    }
}