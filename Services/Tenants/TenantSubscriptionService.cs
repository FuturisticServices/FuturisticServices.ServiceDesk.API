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
    public interface ITenantSubscriptionService
    {
        Task<TenantSubscription> CreateItem(TenantSubscription entity);
    }

    public class TenantSubscriptionService : TenantBaseService, ITenantSubscriptionService
    {
        private readonly ITenantSubscriptionsManager _tenantSubscriptionsManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TenantSubscriptionService(ITenantSubscriptionsManager tenantSubscriptionsManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _tenantSubscriptionsManager = tenantSubscriptionsManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<TenantSubscription> CreateItem(TenantSubscription entity)
        {
            var results = await _tenantSubscriptionsManager.CreateItemAsync(entity);
            return results;
        }
    }
}