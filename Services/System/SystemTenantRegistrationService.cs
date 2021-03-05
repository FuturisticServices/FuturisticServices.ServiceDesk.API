using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServiceDesk.API.Common;
using TangledServices.ServiceDesk.API.Entities;
using TangledServices.ServiceDesk.API.Managers;
using TangledServices.ServiceDesk.API.Models;

namespace TangledServices.ServiceDesk.API.Services
{
    public interface ISystemTenantRegistrationService
    {
        Task<Tenant> Register(RegistrationModel model);
        Task<Tenant> Register(string moniker);
    }

    public class SystemTenantRegistrationService : SystemBaseService, ISystemTenantRegistrationService
    {
        #region Members
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly ISystemSubscriptionService _systemSubscriptionService;
        private readonly ISystemTenantManager _systemTenantManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public SystemTenantRegistrationService(ISystemSubscriptionService systemSubscriptionService, ISystemLookupItemService systemLookupItemService, ISystemTenantManager systemTenantManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemLookupItemService = systemLookupItemService;
            _systemSubscriptionService = systemSubscriptionService;
            _systemTenantManager = systemTenantManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region Public methods
        public async Task<Tenant> Register(RegistrationModel model)
        {
            Tenant tenant = null;

            //  If tenant exists in container, return.
            if (await TenantExists(model.Moniker)) throw new MonikerAlreadyExistsException(model.Moniker.ToUpper());

            //  Get all lookup items.
            List<LookupGroupEntity> systemLookupItems = (await _systemLookupItemService.GetItems()).ToList();

            //  Get current subscription.
            Subscription currentSubscription = await _systemSubscriptionService.GetItem(model.SubscriptionId);

            //  Create and populate tenant object.
            tenant = new Tenant(model, currentSubscription, systemLookupItems);

            tenant = await _systemTenantManager.CreateItemAsync(tenant);

            return tenant;
        }

        public async Task<Tenant> Register(string moniker)
        {
            Tenant tenant = null;

            //  If tenant exists in container, return.
            if (await TenantExists(moniker)) throw new MonikerAlreadyExistsException(moniker.ToUpper());

            //  Get list of tenants from config file.
            var models = _configuration.GetSection("tenants").Get<List<RegistrationModel>>();

            //  If tenant does not exist in config file, return.
            if (!models.Exists(x => string.Compare(x.Moniker, moniker, true) == 0)) throw new MonikerDoesNotExistException(moniker.ToUpper());

            foreach (RegistrationModel registrationModel in models)
            {
                //  Get all lookup items.
                List<LookupGroupEntity> systemLookupItems = (await _systemLookupItemService.GetItems()).ToList();

                //  Get current subscription.
                Subscription currentSubscription = await _systemSubscriptionService.GetItem(registrationModel.SubscriptionId);

                //  Create and populate tenant object.
                tenant = new Tenant(registrationModel, currentSubscription, systemLookupItems);

                tenant = await _systemTenantManager.CreateItemAsync(tenant);

                return tenant;
            }

            return tenant;
        }
        #endregion Public methods

        #region Private methods
        private async Task<bool> TenantExists(string moniker)
        {
            var tenants = await _systemTenantManager.GetItemsAsync();
            var tenantExists = tenants.ToList().Exists(x => string.Compare(x.Moniker, moniker, true) == 0);

            return tenantExists;
        }
        #endregion Private methods
    }
}
