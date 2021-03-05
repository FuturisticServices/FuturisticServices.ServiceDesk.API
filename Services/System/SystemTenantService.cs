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
    public interface ISystemTenantService
    {
        Task<IEnumerable<Tenant>> GetItems();
        Task<Tenant> GetItem(string moniker);
        Task<Tenant> CreateItem(Tenant tenant);
    }

    public class SystemTenantService : SystemBaseService, ISystemTenantService
    {
        #region Members
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly ISystemSubscriptionService _systemSubscriptionService;
        private readonly ISystemTenantManager _systemTenantManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public SystemTenantService(ISystemSubscriptionService systemSubscriptionService, ISystemLookupItemService systemLookupItemService, ISystemTenantManager systemTenantManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemLookupItemService = systemLookupItemService;
            _systemSubscriptionService = systemSubscriptionService;
            _systemTenantManager = systemTenantManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region Public methods
        public async Task<IEnumerable<Tenant>> GetItems()
        {
            IEnumerable<Tenant> results = await _systemTenantManager.GetItemsAsync();

            return results;
        }

        public async Task<Tenant> GetItem(string moniker)
        {
            Tenant results = await _systemTenantManager.GetItemAsync(moniker);

            return results;
        }

        public async Task<Tenant> CreateItem(Tenant tenant)
        {
            Tenant results = await _systemTenantManager.CreateItemAsync(tenant);

            return results;
        }
        #endregion Public methods
    }
}
