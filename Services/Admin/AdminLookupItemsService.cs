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
    public interface IAdminLookupItemsService
    {
        Task<LookupItem> GetItem(Enums.LookupItems item);
        Task<LookupItem> GetItem(string itemName);
        Task<LookupItemValue> GetItem(string itemName, string id);
        Task<IEnumerable<LookupItem>> GetItems();
        Task<LookupItem> CreateItem(LookupItemModel model);
        Task<LookupItem> UpdateGroup(LookupItem model);
    }

    public class AdminLookupItemsService : AdminBaseService, IAdminLookupItemsService
    {
        private readonly IAdminLookupItemsManager _systemLookupItemManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminLookupItemsService(IAdminLookupItemsManager systemLookupItemManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemLookupItemManager = systemLookupItemManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<LookupItem> GetItem(Enums.LookupItems item)
        {
            LookupItem results = await _systemLookupItemManager.GetItemAsync(item);
            return results;
        }

        public async Task<LookupItem> GetItem(string itemName)
        {
            LookupItem results = await _systemLookupItemManager.GetItemAsync(itemName);
            return results;
        }

        public async Task<LookupItemValue> GetItem(string itemName, string id)
        {
            LookupItemValue results = await _systemLookupItemManager.GetItemAsync(itemName, id);
            return results;
        }

        public async Task<IEnumerable<LookupItem>> GetItems()
        {
            var results = await _systemLookupItemManager.GetItemsAsync();
            return results;
        }

        public async Task<LookupItem> CreateItem(LookupItemModel model)
        {
            if (await GetItem(model.Name) == null)
            {
                var entity = new LookupItem(model);
                LookupItem results = await _systemLookupItemManager.CreateItemAsync(entity);
                return results;
            }
            return null;
        }

        public async Task<LookupItem> UpdateGroup(LookupItem model)
        {
            var group = await _systemLookupItemManager.GetItemAsync(model.Name);

            if (group != null)
            {
                group.Values.ToList().ForEach(x => x.Id = x.Id != null ? x.Id : Guid.NewGuid().ToString());
                LookupItem results = await _systemLookupItemManager.UpsertGroupAsync(group);
                return results;
            }

            return null;
        }

        #region Private methods
        private async Task<IEnumerable<LookupItemValue>> ConvertModelToEntity(List<LookupItemValueModel> model)
        {
            List<LookupItemValue> lookupItemsEntity = new List<LookupItemValue>();

            foreach (LookupItemValueModel lookupItemValueModel in model)
            {
                LookupItemValue lookupItemEntity = new LookupItemValue()
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
