using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ITenantSubscriptionsManager
    {
        Task<TenantSubscription> CreateItemAsync(TenantSubscription entity);
    }

    public class TenantSubscriptionsManager : TenantBaseManager, ITenantSubscriptionsManager
    {
        private readonly ISystemTenantsManager _systemTenantsManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public TenantSubscriptionsManager(ISystemTenantsManager systemTenantsManager, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Subscriptions", systemTenantsManager, httpContextAccessor, configuration, webHostEnvironment)
        {
            _systemTenantsManager = systemTenantsManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<TenantSubscription> CreateItemAsync(TenantSubscription entity)
        {
            var results = await _container.CreateItemAsync<TenantSubscription>(entity, new PartitionKey(entity.Subscription.Name));
            return results;
        }
    }
}