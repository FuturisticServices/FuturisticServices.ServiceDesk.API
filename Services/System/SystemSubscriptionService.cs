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
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;
using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ISystemSubscriptionService
    {
        Task<Subscription> GetItem(string id);
        Task<IEnumerable<Subscription>> GetItems();
        Task<IEnumerable<Subscription>> GetItems(bool includeSubscriptionsWithPromotionCode = false);
        Task<Subscription> GetItemByPromotionCode(string promotionCode);
        Task<SubscriptionModel> Validate(SubscriptionModel model);
    }

    public class SystemSubscriptionService : SystemBaseService, ISystemSubscriptionService
    {
        private readonly ISystemSubscriptionManager _systemSubscriptionManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemSubscriptionService(ISystemSubscriptionManager systemSubscriptionManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemSubscriptionManager = systemSubscriptionManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Subscription> GetItem(string id)
        {
            Subscription results = await _systemSubscriptionManager.GetItemAsync(id);

            return results;
        }

        public async Task<IEnumerable<Subscription>> GetItems()
        {
            IEnumerable<Subscription> results = await _systemSubscriptionManager.GetItemsAsync();

            return results;
        }

        public async Task<IEnumerable<Subscription>> GetItems(bool includeSubscriptionsWithPromotionCode = false)
        {
            IEnumerable<Subscription> results = await _systemSubscriptionManager.GetItemsAsync(includeSubscriptionsWithPromotionCode);

            return results;
        }

        public async Task<Subscription> GetItemByPromotionCode(string promotionCode)
        {
            Subscription results = await _systemSubscriptionManager.GetItemByPromotionCodeAsync(promotionCode);

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
