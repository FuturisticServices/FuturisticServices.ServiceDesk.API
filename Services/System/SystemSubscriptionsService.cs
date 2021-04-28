using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ISystemSubscriptionsService
    {
        Task<bool> Found(string subscriptionId);
        Task<bool> NotFound(string subscriptionId);
        Task<Subscription> CreateItem(Subscription subscription);
        Task<Subscription> GetItem(string id);
        Task<IEnumerable<Subscription>> GetItems();
        Task<IEnumerable<Subscription>> GetItems(bool includeSubscriptionsWithPromotionCode = false);
        Task<Subscription> GetItemByPromotionCode(string promotionCode);
        Task<SubscriptionModel> Validate(SubscriptionModel model);
    }

    public class SystemSubscriptionsService : SystemBaseService, ISystemSubscriptionsService
    {
        private readonly ISystemSubscriptionsManager _systemSubscriptionsManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemSubscriptionsService(ISystemSubscriptionsManager systemSubscriptionManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemSubscriptionsManager = systemSubscriptionManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> Found(string subscriptionId)
        {
            Subscription subscription = await _systemSubscriptionsManager.GetItemAsync(subscriptionId);
            return subscription != null;
        }

        public async Task<bool> NotFound(string subscriptionId)
        {
            Subscription subscription = await _systemSubscriptionsManager.GetItemAsync(subscriptionId);
            return subscription == null;
        }

        public async Task<Subscription> CreateItem(Subscription subscription)
        {
            subscription = await _systemSubscriptionsManager.CreateItemAsync(subscription);
            return subscription;
        }

        public async Task<Subscription> GetItem(string id)
        {
            Subscription results = await _systemSubscriptionsManager.GetItemAsync(id);

            return results;
        }

        public async Task<IEnumerable<Subscription>> GetItems()
        {
            IEnumerable<Subscription> results = await _systemSubscriptionsManager.GetItemsAsync();

            return results;
        }

        public async Task<IEnumerable<Subscription>> GetItems(bool includeSubscriptionsWithPromotionCode = false)
        {
            IEnumerable<Subscription> results = await _systemSubscriptionsManager.GetItemsAsync(includeSubscriptionsWithPromotionCode);

            return results;
        }

        public async Task<Subscription> GetItemByPromotionCode(string promotionCode)
        {
            Subscription results = await _systemSubscriptionsManager.GetItemByPromotionCodeAsync(promotionCode);

            return results;
        }

        public async Task<SubscriptionModel> Validate(SubscriptionModel model)
        {
            if (model.Id == string.Empty) throw new SubscriptionIsRequiredException();

            //  Get subscription.
            Subscription subscription = await GetItem(model.Id);
            if (subscription == null) throw new SubscriptionNotFoundException(model.Id, true);

            //  Populate model with subscription.
            model = new SubscriptionModel(subscription);

            return model;
        }
    }
}
