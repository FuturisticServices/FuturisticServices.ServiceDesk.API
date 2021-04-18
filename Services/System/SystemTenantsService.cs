using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ISystemTenantsService
    {
        Task<IEnumerable<SystemTenant>> Get();
        Task<SystemTenant> Get(string moniker);
        Task<bool> Exists(string moniker);
        Task<bool> NotExists(string moniker);
        Task<SystemTenantModel> Create(SystemTenantCreateModel model);
    }

    public class SystemTenantsService : SystemBaseService, ISystemTenantsService
    {
        #region Members
        private readonly ICompanyService _companyService;
        private readonly IPointOfContactService _pointOfContactService;
        private readonly IBillingInformationService _billingInformationService;
        private readonly ISystemLookupItemsService _systemLookupItemService;
        private readonly ISystemSubscriptionService _systemSubscriptionService;
        private readonly ISystemTenantsManager _systemTenantsManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public SystemTenantsService(ICompanyService companyService, IPointOfContactService pointOfContactService, IBillingInformationService billingInformationService, ISystemSubscriptionService systemSubscriptionService, ISystemLookupItemsService systemLookupItemService, ISystemTenantsManager systemTenantsManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _companyService = companyService;
            _pointOfContactService = pointOfContactService;
            _billingInformationService = billingInformationService;
            _systemLookupItemService = systemLookupItemService;
            _systemSubscriptionService = systemSubscriptionService;
            _systemTenantsManager = systemTenantsManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region Public methods
        public async Task<IEnumerable<SystemTenant>> Get()
        {
            IEnumerable<SystemTenant> results = await _systemTenantsManager.GetItemsAsync();

            return results;
        }

        public async Task<SystemTenant> Get(string moniker)
        {
            SystemTenant systemTenant = await _systemTenantsManager.GetItemAsync(moniker);

            if (systemTenant == null) throw new SystemTenantDoesNotExistException(moniker);

            return systemTenant;
        }

        public async Task<bool> Exists(string moniker)
        {
            SystemTenant systemTenant = await _systemTenantsManager.GetItemAsync(moniker);
            return systemTenant != null;
        }

        public async Task<bool> NotExists(string moniker)
        {
            SystemTenant systemTenant = await _systemTenantsManager.GetItemAsync(moniker);
            return systemTenant == null;
        }

        public async Task<SystemTenantModel> Create(SystemTenantCreateModel model)
        {
            model = await Validate(model);

            //  Instantiate system tenant entity object.
            SystemTenant systemTenant = new SystemTenant(model);

            //  Persist system tenant to DB.
            systemTenant = await _systemTenantsManager.CreateItemAsync(systemTenant);

            SystemTenantModel response = new SystemTenantModel(systemTenant);

            return response;
        }

        internal async Task<SystemTenantCreateModel> Validate(SystemTenantCreateModel model)
        {
            model.Id = Guid.NewGuid().ToString();

            if (model.Moniker == null || model.Moniker == string.Empty) throw new MonikerIsRequiredException();
            if (await Exists(model.Moniker)) throw new MonikerAlreadyExistsException();

            model.Subscription = await _systemSubscriptionService.Validate(model.Subscription);
            model.Company = await _companyService.Validate(model.Company);
            model.PointOfContact = await _pointOfContactService.Validate(model.PointOfContact);
            model.BillingInformation = await _billingInformationService.Validate(model.BillingInformation);

            return model;
        }
        #endregion Public methods
    }
}
