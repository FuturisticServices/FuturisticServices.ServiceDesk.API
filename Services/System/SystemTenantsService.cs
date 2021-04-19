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
        Task<SystemTenant> Get(Guid id);
        Task<SystemTenant> Get(string moniker);
        Task<bool> NotExists(Guid id);
        Task<bool> Exists(string moniker);
        Task<bool> NotExists(string moniker);
        Task<bool> Exists(Guid id);
        Task<SystemTenantModel> Create(SystemTenantModel model);
        Task<SystemTenantModel> Update(SystemTenantModel model);
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

        public async Task<SystemTenant> Get(Guid id)
        {
            SystemTenant systemTenant = await _systemTenantsManager.GetItemAsync(id);

            if (systemTenant == null) throw new SystemTenantDoesNotExistException();

            return systemTenant;
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

        public async Task<bool> Exists(Guid id)
        {
            SystemTenant systemTenant = await _systemTenantsManager.GetItemAsync(id);
            return systemTenant != null;
        }

        public async Task<bool> NotExists(Guid id)
        {
            SystemTenant systemTenant = await _systemTenantsManager.GetItemAsync(id);
            return systemTenant == null;
        }

        public async Task<SystemTenantModel> Create(SystemTenantModel model)
        {
            model = await Validate(model);

            //  Create SystemTenant entity object.
            SystemTenant systemTenant = new SystemTenant(model);
            systemTenant.Id = Guid.NewGuid().ToString();

            //  Persist system tenant to DB.
            systemTenant = await _systemTenantsManager.CreateItemAsync(systemTenant);

            SystemTenantModel response = new SystemTenantModel(systemTenant);

            return response;
        }

        public async Task<SystemTenantModel> Update(SystemTenantModel model)
        {
            model = await Validate(model);

            //  Get SystemTenant entity to update.
            SystemTenant systemTenant = await Get(new Guid(model.Id));

            //  Update SystemTenant entity object with modifications from model.
            systemTenant = new SystemTenant(model);

            //  Persist system tenant to DB.
            systemTenant = await _systemTenantsManager.UpserItemAsync(systemTenant);

            SystemTenantModel response = new SystemTenantModel(systemTenant);

            return response;
        }
        #endregion Public methods

        #region Private methods
        private async Task<SystemTenantModel> Validate(SystemTenantModel model)
        {
            if (model.Moniker == null || model.Moniker == string.Empty) throw new MonikerIsRequiredException();

            if (model.Id == null)
            {
                if (await Exists(model.Moniker)) throw new MonikerAlreadyExistsException();
            }
            else
            {
                if (await NotExists(new Guid(model.Id))) throw new SystemTenantDoesNotExistException();
            }

            model.Subscription = await _systemSubscriptionService.Validate(model.Subscription);
            model.Company = await _companyService.Validate(model.Company);
            model.PointOfContact = await _pointOfContactService.Validate(model.PointOfContact);
            model.BillingInformation = await _billingInformationService.Validate(model.BillingInformation);

            return model;
        }
        #endregion Private methods
    }
}
