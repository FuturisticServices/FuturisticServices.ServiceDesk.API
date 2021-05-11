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
    public interface ITenantLookupItemsService
    {
        Task<SystemLookupItem> GetItem(Enums.LookupItems item);
        Task<SystemLookupItem> GetItem(string itemName);
        Task<SystemLookupItemValue> GetItem(string itemName, string id);
        Task<IEnumerable<SystemLookupItem>> GetItems();
        Task<SystemLookupItem> CreateItem(SystemLookupItemModel model);
        Task<SystemLookupItem> UpdateGroup(SystemLookupItem model);
    }

    public class TenantLookupItemsService : TenantBaseService, ITenantLookupItemsService
    {
        private readonly ITenantLookupItemsManager _systemLookupItemManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TenantLookupItemsService(ITenantLookupItemsManager systemLookupItemManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemLookupItemManager = systemLookupItemManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<SystemLookupItem> GetItem(Enums.LookupItems item)
        {
            SystemLookupItem results = await _systemLookupItemManager.GetItemAsync(item);
            return results;
        }

        public async Task<SystemLookupItem> GetItem(string itemName)
        {
            SystemLookupItem results = await _systemLookupItemManager.GetItemAsync(itemName);
            return results;
        }

        public async Task<SystemLookupItemValue> GetItem(string itemName, string id)
        {
            SystemLookupItemValue results = await _systemLookupItemManager.GetItemAsync(itemName, id);
            return results;
        }

        public async Task<IEnumerable<SystemLookupItem>> GetItems()
        {
            var results = await _systemLookupItemManager.GetItemsAsync();
            return results;
        }

        public async Task<SystemLookupItem> CreateItem(SystemLookupItemModel model)
        {
            if (await GetItem(model.Name) == null)
            {
                SystemLookupItem lookupGroupEntity = new SystemLookupItem()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    Values = await ConvertModelToEntity(model.Values.ToList())
                };

                SystemLookupItem results = await _systemLookupItemManager.CreateItemAsync(lookupGroupEntity);
                return results;
            }
            return null;
        }

        public async Task<SystemLookupItem> UpdateGroup(SystemLookupItem model)
        {
            var group = await _systemLookupItemManager.GetItemAsync(model.Name);

            if (group != null)
            {
                group.Values.ToList().ForEach(x => x.Id = x.Id != null ? x.Id : Guid.NewGuid().ToString());
                SystemLookupItem results = await _systemLookupItemManager.UpsertGroupAsync(group);
                return results;
            }

            return null;
        }

        #region Private methods
        private async Task<IEnumerable<SystemLookupItemValue>> ConvertModelToEntity(List<SystemLookupItemValueModel> model)
        {
            List<SystemLookupItemValue> lookupItemsEntity = new List<SystemLookupItemValue>();

            foreach (SystemLookupItemValueModel lookupItemValueModel in model)
            {
                SystemLookupItemValue lookupItemEntity = new SystemLookupItemValue()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = lookupItemValueModel.Name,
                    Abbreviation = lookupItemValueModel.Abbreviation,
                };

                lookupItemsEntity.Add(lookupItemEntity);
            }

            return lookupItemsEntity;
        }
        #endregion Private methods
    }
}
