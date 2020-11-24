﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

using AutoMapper;
using Newtonsoft.Json;

using FuturisticServices.ServiceDesk.API.Entities;
using FuturisticServices.ServiceDesk.API.Services;
using FuturisticServices.ServiceDesk.API.Common;

namespace FuturisticServices.ServiceDesk.API.Controllers
{
    [Route("api/tenant/setup")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TenantSetupController : ControllerBase
    {
        #region Members
        private readonly ITenantSetupService _tenantSetupService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public TenantSetupController(ITenantSetupService tenantSetupService,
                                IMapper mapper,
                                IConfiguration configuration,
                                IWebHostEnvironment webHostEnvironment)
        {
            _tenantSetupService = tenantSetupService;
            _mapper = mapper;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region Public methods
        /// <summary>
        /// Create tenant database and containers.
        /// </summary>
        /// <param name="moniker">Unique tenant ID used when invoking API calls.</param>
        /// <returns></returns>
        [HttpPost("servicedesk/{moniker}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Setup(string moniker)
        {
            try
            {
                var response = await _tenantSetupService.ServiceDesk(moniker);
                var json = JsonConvert.SerializeObject(response);
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return BadRequest("Error setting up tenant service desk. Error: " + ex.Message);
            }
        }
        #endregion Public methods
    }
}