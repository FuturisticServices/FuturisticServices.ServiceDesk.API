using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Managers.System;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ITenantSubscriptionsManager
    {
        Task<TenantSubscription> CreateItemAsync(TenantSubscription entity);
    }

    public class TenantSubscriptionsManager : TenantBaseManager, ITenantSubscriptionsManager
    {
        private readonly ICustomerManager _customerManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public TenantSubscriptionsManager(ICustomerManager customerManager, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Subscriptions", customerManager, httpContextAccessor, configuration, webHostEnvironment)
        {
            _customerManager = customerManager;
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