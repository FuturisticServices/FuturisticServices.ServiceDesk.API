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
        Task<AdminLookupItem> GetItem(Enums.LookupItems item);
        Task<AdminLookupItem> GetItem(string itemName);
        Task<AdminLookupItemValue> GetItem(string itemName, string id);
        Task<IEnumerable<AdminLookupItem>> GetItems();
        Task<AdminLookupItem> CreateItem(AdminLookupItemModel model);
        Task<AdminLookupItem> UpdateGroup(AdminLookupItem model);
    }

    public class AdminLookupItemsService : AdminBaseService, IAdminLookupItemsService
    {
        private readonly IAdminLookupItemsManager _adminLookupItemManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminLookupItemsService(IAdminLookupItemsManager systemLookupItemManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _adminLookupItemManager = systemLookupItemManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<AdminLookupItem> GetItem(Enums.LookupItems item)
        {
            AdminLookupItem results = await _adminLookupItemManager.GetItemAsync(item);
            return results;
        }

        public async Task<AdminLookupItem> GetItem(string itemName)
        {
            AdminLookupItem results = await _adminLookupItemManager.GetItemAsync(itemName);
            return results;
        }

        public async Task<AdminLookupItemValue> GetItem(string itemName, string id)
        {
            AdminLookupItemValue results = await _adminLookupItemManager.GetItemAsync(itemName, id);
            return results;
        }

        public async Task<IEnumerable<AdminLookupItem>> GetItems()
        {
            var results = await _adminLookupItemManager.GetItemsAsync();
            return results;
        }

        public async Task<AdminLookupItem> CreateItem(AdminLookupItemModel model)
        {
            if (await GetItem(model.Name) == null)
            {
                var entity = new AdminLookupItem(model);
                AdminLookupItem results = await _adminLookupItemManager.CreateItemAsync(entity);
                return results;
            }
            return null;
        }

        public async Task<AdminLookupItem> UpdateGroup(AdminLookupItem model)
        {
            var group = await _adminLookupItemManager.GetItemAsync(model.Name);

            if (group != null)
            {
                group.Values.ToList().ForEach(x => x.Id = x.Id != null ? x.Id : Guid.NewGuid().ToString());
                AdminLookupItem results = await _adminLookupItemManager.UpsertGroupAsync(group);
                return results;
            }

            return null;
        }

        #region Private methods
        private async Task<IEnumerable<AdminLookupItemValue>> ConvertModelToEntity(List<AdminLookupItemValueModel> model)
        {
            List<AdminLookupItemValue> lookupItemsEntity = new List<AdminLookupItemValue>();

            foreach (AdminLookupItemValueModel lookupItemValueModel in model)
            {
                AdminLookupItemValue lookupItemEntity = new AdminLookupItemValue()
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
