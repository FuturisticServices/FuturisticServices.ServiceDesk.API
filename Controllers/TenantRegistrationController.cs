using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using AutoMapper;

using TangledServices.ServiceDesk.API.Common;
using TangledServices.ServiceDesk.API.Entities;
using TangledServices.ServiceDesk.API.Managers;
using TangledServices.ServiceDesk.API.Models;
using TangledServices.ServiceDesk.API.Services;

namespace TangledServices.ServiceDesk.API.Controllers
{
    [Route("api/tenant/registration")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TenantRegistrationController : ControllerBase
    {
        #region Members
        private readonly ISystemTenantService _systemTenantService;
        private readonly ISystemTenantRegistrationService _systemTenantRegistrationService;
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly ISystemSubscriptionService _systemSubscriptionService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        /// <summary>
        /// Manages all endpoints for the registration process.
        /// </summary>
        /// <param name="systemTenantService"></param>
        /// <param name="systemTenantRegistrationService"></param>
        /// <param name="systemLookupItemService"></param>
        /// <param name="systemSubscriptionService"></param>
        /// <param name="mapper"></param>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        public TenantRegistrationController(ISystemTenantService systemTenantService, 
                                        ISystemTenantRegistrationService systemTenantRegistrationService,
                                        ISystemLookupItemService systemLookupItemService, 
                                        ISystemSubscriptionService systemSubscriptionService, 
                                        IMapper mapper, 
                                        IConfiguration configuration, 
                                        IWebHostEnvironment webHostEnvironment)
        {
            _systemTenantService = systemTenantService;
            _systemTenantRegistrationService = systemTenantRegistrationService;
            _systemLookupItemService = systemLookupItemService;
            _systemSubscriptionService = systemSubscriptionService;
            _mapper = mapper;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region GET methods
        /// <summary>
        /// Retrieves all data required to populate controls on the Tenant registration form (ie. states, countries, address types, etc).
        /// </summary>
        /// <returns>HttpStatus 401 ~ Unauthorized</returns>
        /// <returns>HttpStatus 200 ~ Success</returns>
        /// <returns>HttpStatus 400 ~ Bad request</returns>
        [HttpGet("metadata")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MetaData()
        {
            dynamic response = new ExpandoObject();

            try
            {
                //  Get all lookup items.
                List<LookupGroupEntity> systemLookupItems = (await _systemLookupItemService.GetItems()).ToList();
                List<LookupItemEntity> addressTypes = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.AddressTypes.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
                List<LookupItemEntity> phoneNumberTypes = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.PhoneNumberTypes.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
                List<LookupItemEntity> emailAddressTypes = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.EmailAddressTypes.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
                List<LookupItemEntity> websiteTypes = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.WebsiteTypes.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
                List<LookupItemEntity> states = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.States.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
                List<LookupItemEntity> countries = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.Countries.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();

                RegistrationMetaDataModel model = new RegistrationMetaDataModel();
                model.States = states;
                model.Countries = countries;
                model.Subscriptions = (await _systemSubscriptionService.GetItems(false)).ToList();

                response.status = this.StatusCode(StatusCodes.Status200OK, "Registration meta-data retrieved successfully.");
                response.metaData = model;
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                response.status = this.StatusCode(StatusCodes.Status400BadRequest, "Error retrieving registration meta-data. Error: " + ex.Message);
                response.items = null;
                return BadRequest(new { response });
            }
        }

        /// <summary>
        /// Validates the provided code against existing subscriptions in the [TangledServices.ServiceDesk] database Subscriptions container.
        /// Success ~ Returns associated subscription if subscription is valid.
        /// Failure ~ No subscription found associated to promotion code.
        /// </summary>
        /// <param name="promotionCode">10 alphanumeric characters.</param>
        /// <returns>HttpStatus 401 ~ Unauthorized</returns>
        /// <returns>HttpStatus 200 ~ Success</returns>
        /// <returns>HttpStatus 400 ~ Bad request</returns>
        [HttpGet("promotioncode/{promotionCode}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PromotionCode([Required, MinLength(10), MaxLength(10)] string promotionCode)
        {
            dynamic response = new ExpandoObject();

            try
            {
                Subscription subscription = await _systemSubscriptionService.GetItemByPromotionCode(promotionCode);

                if (subscription != null)
                {
                    response.statusCode = this.StatusCode(StatusCodes.Status200OK, subscription != null ? "Subscription associated to promotion code found." : "Subscription associated to promotion code NOT found.");
                    response.subscription = subscription;
                    return Ok(new { response });
                }

                response.statusCode = this.StatusCode(StatusCodes.Status400BadRequest, "No Subscriptions found associated to promotion code.");
                response.subscription = subscription;
                return BadRequest(new { response });
            }
            catch (Exception ex)
            {
                response.status = this.StatusCode(StatusCodes.Status400BadRequest, "Error retrieving subscription associated with promotion code. Error: " + ex.Message);
                response.subscription = null;
                return BadRequest(new { response });
            }
        }

        /// <summary>
        /// Determines if the provided moniker is available for use.
        /// </summary>
        /// <param name="moniker">3-10 alphanumeric characters.</param>
        /// <returns>HttpStatus 401 ~ Unauthorized</returns>
        /// <returns>HttpStatus 200 ~ Success</returns>
        /// <returns>HttpStatus 400 ~ Bad Request</returns>
        [HttpGet("moniker/{moniker}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Moniker(string moniker)
        {
            dynamic response = new ExpandoObject();

            try
            {
                Tenant tenant = await _systemTenantService.GetItem(moniker);
                bool monikerIsUnique = tenant == null ? true : false;

                response.statusCode = StatusCode(StatusCodes.Status200OK, monikerIsUnique ? "Moniker is unique." : "Moniker is NOT unique.");
                response.moniker = moniker;
                response.monikerIsUnique = monikerIsUnique;
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                response.status = this.StatusCode(StatusCodes.Status400BadRequest, "Error retrieving subscription associated with promotion code. Error: " + ex.Message);
                response.items = null;
                return BadRequest(new { response });
            }
        }
        #endregion GET methods

        #region POST methods
        /// <summary>
        /// Creates a new tenant within the [TangledServices.ServiceDesk] database Tenant container.
        /// </summary>
        /// <param name="model">RegistrationModel view model.</param>
        /// <returns>HttpStatus 401 ~ Unauthorized</returns>
        /// <returns>HttpStatus 200 ~ Success</returns>
        /// <returns>HttpStatus 400 ~ Bad Request</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Tenant([FromBody] RegistrationModel model)
        {
            dynamic response = new ExpandoObject();

            try
            {
                Tenant tenant = await _systemTenantRegistrationService.Register(model);

                if (tenant != null)
                {
                    response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("Tenant '{0}' registered successfully.", model.Moniker.ToUpper()));
                    response.tenant = tenant;
                    return Ok(new { response });
                }

                response.status = this.StatusCode(StatusCodes.Status400BadRequest, "Error registering tenant.");
                return BadRequest(new { response });
            }
            catch (Exception ex)
            {
                response.status = this.StatusCode(StatusCodes.Status400BadRequest, "Error registering tenant. Error: " + ex.Message);
                return BadRequest(new { response });
            }
        }

        /// <summary>
        /// Registers a tenant with the matching moniker from the tenants.json file.
        /// </summary>
        /// <param name="moniker"></param>
        /// <returns>HTTPStatus 200 OK ~ success</returns>
        /// <returns>HTTPStatus 400 BadRequest ~ failure</returns>
        [HttpPost("{moniker}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Tenant(string moniker)
        {
            dynamic response = new ExpandoObject();

            try
            {
                Tenant tenant = (await _systemTenantRegistrationService.Register(moniker));

                if (tenant != null)
                {
                    response.status = this.StatusCode(StatusCodes.Status200OK, string.Format("Tenant '{0}' registered successfully.", moniker.ToUpper()));
                    response.tenant = tenant;
                    return Ok(new { response });
                }

                response.status = this.StatusCode(StatusCodes.Status400BadRequest, "Error registering tenant.");
                return BadRequest(new { response });
            }
            catch (Exception ex)
            {
                response.status = this.StatusCode(StatusCodes.Status400BadRequest, "Error registering tenant. Error: " + ex.Message);
                return BadRequest(new { response });
            }
        }
        #endregion POST methods
    }
}