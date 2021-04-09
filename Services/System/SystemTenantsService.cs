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
        Task<bool> NotExists(string moniker);
        Task<SystemTenant> Create(SystemTenantModel model);
    }

    public class SystemTenantsService : SystemBaseService, ISystemTenantsService
    {
        #region Members
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly ISystemSubscriptionService _systemSubscriptionService;
        private readonly ISystemTenantsManager _systemTenantsManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public SystemTenantsService(ISystemSubscriptionService systemSubscriptionService, ISystemLookupItemService systemLookupItemService, ISystemTenantsManager systemTenantsManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
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

        public async Task<bool> NotExists(string moniker)
        {
            SystemTenant systemTenant = await _systemTenantsManager.GetItemAsync(moniker);
            return systemTenant == null;
        }

        public async Task<SystemTenant> Create(SystemTenantModel model)
        {
            //  Cannot create tenant if it already exists in the system DB.
            if (await NotExists(model.Moniker)) throw new SystemTenantAlreadyExistsException(model.Moniker);

            //  Get subscription.
            Subscription subscription = await _systemSubscriptionService.GetItem(model.SubscriptionId);
            if (subscription == null) throw new SubscriptionNotFoundException(model.SubscriptionId, true);

            //  Get list of system lookup items.
            List<LookupGroupEntity> systemLookupItems = (await _systemLookupItemService.GetItems()).ToList();
            if (systemLookupItems == null) throw new LookupItemsNotFoundException(true);

            //  Instantiate system tenant entity object.
            SystemTenant systemTenant = new SystemTenant(model, subscription, systemLookupItems);

            //  Persist system tenant to DB.
            SystemTenant results = await _systemTenantsManager.CreateItemAsync(systemTenant);

            return results;
        }
        #endregion Public methods
    }
}
