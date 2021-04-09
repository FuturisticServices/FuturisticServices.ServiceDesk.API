using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Managers;
using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ISystemTenantRegistrationService
    {
        Task<SystemTenant> Register(RegistrationModel model);
        Task<SystemTenant> Register(string moniker);
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
        public async Task<SystemTenant> Register(SystemTenantModel model)
        {
            SystemTenant tenant = null;

            //  If tenant exists in container, return.
            if (await TenantExists(model.Moniker)) throw new MonikerAlreadyExistsException(model.Moniker.ToUpper());

            //  Get all lookup items.
            List<LookupGroupEntity> systemLookupItems = (await _systemLookupItemService.GetItems()).ToList();

            //  Get current subscription.
            Subscription currentSubscription = await _systemSubscriptionService.GetItem(model.SubscriptionId);

            //  Create and populate tenant object.
            tenant = new SystemTenant(model, currentSubscription, systemLookupItems);

            tenant = await _systemTenantManager.CreateItemAsync(tenant);

            return tenant;
        }

        public async Task<SystemTenant> Register(string moniker)
        {
            SystemTenant tenant = null;

            //  If tenant exists in container, return.
            if (await TenantExists(moniker)) throw new MonikerAlreadyExistsException(moniker.ToUpper());

            //  Get list of tenants from config file.
            var models = _configuration.GetSection("tenants").Get<List<SystemTenantModel>>();

            //  If tenant does not exist in config file, return.
            if (!models.Exists(x => string.Compare(x.Moniker, moniker, true) == 0)) throw new MonikerDoesNotExistException(moniker.ToUpper());

            foreach (SystemTenantModel registrationModel in models)
            {
                //  Get all lookup items.
                List<LookupGroupEntity> systemLookupItems = (await _systemLookupItemService.GetItems()).ToList();

                //  Get current subscription.
                Subscription currentSubscription = await _systemSubscriptionService.GetItem(registrationModel.SubscriptionId);

                //  Create and populate tenant object.
                tenant = new SystemTenant(registrationModel, currentSubscription, systemLookupItems);

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
