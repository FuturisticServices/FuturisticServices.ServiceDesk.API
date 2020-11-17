using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using AutoMapper;

using FuturisticServices.ServiceDesk.API.Common;
using FuturisticServices.ServiceDesk.API.Entities;
using FuturisticServices.ServiceDesk.API.Managers;
using FuturisticServices.ServiceDesk.API.Models;
using System.Dynamic;

namespace FuturisticServices.ServiceDesk.API.Controllers
{
    [Route("api/registration")]
    [ApiVersion("1.0")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ISystemTenantsManager _systemTenantsService;
        private readonly ISystemLookupItemsManager _systemLookupItemsService;
        private readonly ISystemSubscriptionsManager _systemSubscriptionsService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Manages all endpoints for the registration process.
        /// </summary>
        /// <param name="systemTenantsService"></param>
        /// <param name="systemLookupItemsService"></param>
        /// <param name="systemSubscriptionsService"></param>
        /// <param name="mapper"></param>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        public RegistrationController(ISystemTenantsManager systemTenantsService,
                                        ISystemLookupItemsManager systemLookupItemsService, 
                                        ISystemSubscriptionsManager systemSubscriptionsService, 
                                        IMapper mapper, 
                                        IConfiguration configuration, 
                                        IWebHostEnvironment webHostEnvironment)
        {
            _systemTenantsService = systemTenantsService;
            _systemLookupItemsService = systemLookupItemsService;
            _systemSubscriptionsService = systemSubscriptionsService;
            _mapper = mapper;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Retrieves all necessary data required to populate the registration input elements upon rendering.
        /// </summary>
        /// <returns>HttpStatus 200 ~ Success</returns>
        /// <returns>HttpStatus 400 ~ Bad request</returns>
        [HttpGet("metadata")]
        [AllowAnonymous]
        public async Task<IActionResult> MetaData()
        {
            dynamic response = new ExpandoObject();

            try
            {
                //  Get all lookup items.
                List<LookupGroup> systemLookupItems = (await _systemLookupItemsService.GetItemsAsync()).ToList();
                List<LookupItem> addressTypes = systemLookupItems.Where(x => x.Name == Enums.LookupGroups.AddressTypes.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
                List<LookupItem> phoneNumberTypes = systemLookupItems.Where(x => x.Name == Enums.LookupGroups.PhoneNumberTypes.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
                List<LookupItem> emailAddressTypes = systemLookupItems.Where(x => x.Name == Enums.LookupGroups.EmailAddressTypes.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
                List<LookupItem> websiteTypes = systemLookupItems.Where(x => x.Name == Enums.LookupGroups.WebsiteTypes.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
                List<LookupItem> states = systemLookupItems.Where(x => x.Name == Enums.LookupGroups.States.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
                List<LookupItem> countries = systemLookupItems.Where(x => x.Name == Enums.LookupGroups.Countries.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();

                RegistrationMetaDataModel model = new RegistrationMetaDataModel();
                model.States = states;
                model.Countries = countries;
                model.Subscriptions = (await _systemSubscriptionsService.GetItemsAsync(false)).ToList();

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
        /// Creates a new tenant within the FuturisticServices.ServiceDesk-->Tenant container.
        /// </summary>
        /// <param name="model">RegistrationModel view model.</param>
        /// <returns>HttpStatus 200 ~ Success</returns>
        /// <returns>HttpStatus 400 ~ Bad Request</returns>
        [HttpPost("tenant")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Tenant([FromBody] RegistrationModel model)
        {
            dynamic response = new ExpandoObject();

            try
            {
                //  Get all lookup items.
                List<LookupGroup> systemLookupItems = (await _systemLookupItemsService.GetItemsAsync()).ToList();

                //  Get current subscription.
                Subscription currentSubscription = await _systemSubscriptionsService.GetItemAsync(model.SubscriptionId);

                //  Create and populate tenant object.
                Tenant tenant = new Tenant(model, currentSubscription, systemLookupItems);

                tenant = await _systemTenantsService.CreateItemAsync(tenant);

                response.status = this.StatusCode(StatusCodes.Status200OK, "Tenant registered successfully.");
                response.tenant = tenant;
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                response.status = this.StatusCode(StatusCodes.Status400BadRequest, "Error registering tenant. Error: " + ex.Message);
                response.items = null;
                return BadRequest(new { response });
            }
        }

        /// <summary>
        /// Validates the provided code against existing subscriptions.
        /// Success ~ Returns associated subscription if subscription is valid.
        /// Failure ~ No subscription found associated to promotion code.
        /// </summary>
        /// <param name="promotionCode">10 alphanumeric characters.</param>
        /// <returns></returns>
        [HttpGet("promotioncode/{promotionCode}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PromotionCode([Required, MinLength(10), MaxLength(10)] string promotionCode)
        {
            dynamic response = new ExpandoObject();

            try
            {
                Subscription subscription = await _systemSubscriptionsService.GetItemByPromotionCodeAsync(promotionCode);

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
        /// Retrieves information required to populate the registration wizard.
        /// </summary>
        /// <param name="moniker">3-10 alphanumeric characters.</param>
        /// <returns></returns>
        [HttpGet("moniker/{moniker}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Moniker(string moniker)
        {
            dynamic response = new ExpandoObject();

            try
            {
                Tenant tenant = await _systemTenantsService.GetItemAsync(moniker);
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
    }
}