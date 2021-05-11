using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Managers;
using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ISystemLookupItemService
    {
        Task<SystemLookupItemModel> GetItem(Guid id);
        Task<SystemLookupItemModel> GetItem(string name);
        Task<SystemLookupItemModel> GetItem(Enums.LookupItems item);
        Task<SystemLookupItemModel> GetItem(Enums.LookupItems item, string id);
        Task<SystemLookupItemValueModel> GetItem(string name, string id);
        Task<IEnumerable<SystemLookupItemModel>> GetItems();
        Task<SystemLookupItemModel> CreateItem(SystemLookupItemModel model);
        Task<SystemLookupItemModel> Update(SystemLookupItemModel model);

        //Task<SystemLookupItemModel> DeleteItem(Guid groupId, Guid itemId);

        //Task<SystemLookupItemModel> DeleteItem(string groupName, Guid itemId);

        //Task<SystemLookupItemModel> DeleteItem(Guid groupId, string itemName);

        //Task<SystemLookupItemModel> DeleteItem(string groupName, string itemName);
        Task<SystemLookupItemModel> DeleteItem(string groupKey, string itemKey);
    }

    public class SystemLookupItemService : SystemBaseService, ISystemLookupItemService
    {
        private readonly ISystemLookupItemManager _systemLookupItemManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemLookupItemService(ISystemLookupItemManager systemLookupItemManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemLookupItemManager = systemLookupItemManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<SystemLookupItemModel> GetItem(Guid id)
        {
            var group = await _systemLookupItemManager.GetItemAsync(id);
            if (group == null) throw new SystemLookupItemNotFoundException();

            var model = new SystemLookupItemModel(group);
            return model;
        }

        public async Task<SystemLookupItemModel> GetItem(string name)
        {
            var group = await _systemLookupItemManager.GetItemAsync(name);
            if (group == null) return null;

            var model = new SystemLookupItemModel(group);
            return model;
        }

        public async Task<SystemLookupItemModel> GetItem(Enums.LookupItems item)
        {
            var entity = await _systemLookupItemManager.GetItemAsync(item);
            var model = new SystemLookupItemModel(entity);
            return model;
        }

        public async Task<SystemLookupItemModel> GetItem(Enums.LookupItems item, string id)
        {
            SystemLookupItem entity = await _systemLookupItemManager.GetItemAsync(item, id);
            var model = new SystemLookupItemModel(entity);
            return model;
        }

        public async Task<SystemLookupItemValueModel> GetItem(string name, string id)
        {
            SystemLookupItemValue entity = await _systemLookupItemManager.GetItemAsync(name, id);
            var model = new SystemLookupItemValueModel(entity);
            return model;
        }

        public async Task<IEnumerable<SystemLookupItemModel>> GetItems()
        {
            var entitites = await _systemLookupItemManager.GetItemsAsync();
            if (!entitites.Any()) throw new SystemLookupItemsNotFoundException();

            var model = SystemLookupItemModel.Construct(entitites);
            return model;
        }

        public async Task<SystemLookupItemModel> CreateItem(SystemLookupItemModel model)
        {
            if (await GetItem(model.Name) != null) throw new SystemLookupItemAlreadyExistsException(model.Name);

            SystemLookupItem entity = new SystemLookupItem(model);
            entity = await _systemLookupItemManager.CreateItemAsync(entity);
            model = new SystemLookupItemModel(entity);
            return model;
        }

        public async Task<SystemLookupItemModel> Update(SystemLookupItemModel model)
        {
            var entity = await _systemLookupItemManager.GetItemAsync(new Guid(model.Id));
            if (entity == null) throw new SystemLookupItemNotFoundException();

            entity = new SystemLookupItem(model);
            entity = await _systemLookupItemManager.UpsertGroupAsync(entity);
            model = new SystemLookupItemModel(entity);
            return model;
        }

        //public async Task<SystemLookupItemModel> DeleteItem(Guid groupId, Guid itemId)
        //{
        //    var group = await _systemLookupItemManager.GetItemAsync(groupId);
        //    if (group == null) throw new SystemLookupItemNotFoundException();

        //    var item = group.Values.SingleOrDefault(x => x.Id == itemId.ToString());
        //    if (item == null) throw new SystemLookupItemNotFoundException();

        //    item.Enabled = false;

        //    var model = new SystemLookupItemModel(group);
        //    return model;
        //}

        //public async Task<SystemLookupItemModel> DeleteItem(string groupName, Guid itemId)
        //{
        //    var group = await _systemLookupItemManager.GetItemAsync(groupName);
        //    if (group == null) throw new SystemLookupItemNotFoundException();

        //    var item = group.Values.SingleOrDefault(x => x.Id == itemId.ToString());
        //    if (item == null) throw new SystemLookupItemNotFoundException();

        //    item.Enabled = false;

        //    var model = new SystemLookupItemModel(group);
        //    return model;
        //}

        //public async Task<SystemLookupItemModel> DeleteItem(Guid groupId, string itemName)
        //{
        //    var group = await _systemLookupItemManager.GetItemAsync(groupId);
        //    if (group == null) throw new SystemLookupItemNotFoundException();

        //    var item = group.Values.SingleOrDefault(x => x.Name == itemName);
        //    if (item == null) throw new SystemLookupItemNotFoundException();

        //    item.Enabled = false;

        //    var model = new SystemLookupItemModel(group);
        //    return model;
        //}

        //public async Task<SystemLookupItemModel> DeleteItem(string groupName, string itemName)
        //{
        //    var group = await _systemLookupItemManager.GetItemAsync(groupName);
        //    if (group == null) throw new SystemLookupItemNotFoundException();

        //    var item = group.Values.SingleOrDefault(x => x.Name == itemName);
        //    if (item == null) throw new SystemLookupItemNotFoundException();

        //    item.Enabled = false;

        //    var model = new SystemLookupItemModel(group);
        //    return model;
        //}

        public async Task<SystemLookupItemModel> DeleteItem(string groupKey, string itemKey)
        {
            var groupKeyIsGuid = Guid.TryParse(groupKey, out var groupId);
            var itemKeyIsGuid = Guid.TryParse(itemKey, out var itemId);

            var group = new SystemLookupItem();
            if (groupKeyIsGuid)
            {
                group = await _systemLookupItemManager.GetItemAsync(groupId);
                if (group == null) throw new SystemLookupItemNotFoundException(string.Format("System LookupItem group ID '{0}', item ID '{1}' deleted successfully.", groupId, itemId));
            }
            else
            {
                group = await _systemLookupItemManager.GetItemAsync(groupKey);
                if (group == null) throw new SystemLookupItemNotFoundException(string.Format("System LookupItem group '{0}', item ID '{1}' deleted successfully.", groupKey, itemId));
            }

            var item = new SystemLookupItemValue();
            if (itemKeyIsGuid)
            {
                item = group.Values.SingleOrDefault(x => x.Id == itemKey);
                if (item == null) throw new SystemLookupItemNotFoundException(string.Format("System LookupItem group ID '{0}', item '{1}' deleted successfully.", groupId, itemKey));
            }
            else
            {
                item = group.Values.SingleOrDefault(x => x.Name == itemKey);
                if (group == null) throw new SystemLookupItemNotFoundException(string.Format("System LookupItem group '{0}', item '{1}' deleted successfully.", groupKey, itemKey));
            }

            item.Enabled = false;

            var model = new SystemLookupItemModel(group);
            return model;
        }
    }
}
