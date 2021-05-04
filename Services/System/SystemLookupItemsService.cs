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
    public interface ISystemLookupItemsService
    {
        Task<SystemLookupItemModel> GetItem(Enums.LookupItems item);
        Task<SystemLookupItemModel> GetItem(string name);
        Task<SystemLookupItemValueModel> GetItem(string name, string id);
        Task<IEnumerable<SystemLookupItemModel>> GetItems();
        Task<SystemLookupItemModel> CreateItem(SystemLookupItemModel model);
        Task<SystemLookupItemModel> Update(SystemLookupItemModel model);
    }

    public class SystemLookupItemsService : SystemBaseService, ISystemLookupItemsService
    {
        private readonly ISystemLookupItemsManager _systemLookupItemManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemLookupItemsService(ISystemLookupItemsManager systemLookupItemManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemLookupItemManager = systemLookupItemManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<SystemLookupItemModel> GetItem(Enums.LookupItems item)
        {
            var entity = await _systemLookupItemManager.GetItemAsync(item);
            var model = new SystemLookupItemModel(entity);
            return model;
        }

        public async Task<SystemLookupItemModel> GetItem(string name)
        {
            var entity = await _systemLookupItemManager.GetItemAsync(name);
            return entity == null ? null : new SystemLookupItemModel(entity);
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
            var model = SystemLookupItemModel.Construct(entitites);
            return model;
        }

        public async Task<SystemLookupItemModel> CreateItem(SystemLookupItemModel model)
        {
            if (await GetItem(model.Name) == null)
            {
                SystemLookupItem entity = new SystemLookupItem(model);
                entity = await _systemLookupItemManager.CreateItemAsync(entity);
                model = new SystemLookupItemModel(entity);
                return model;
            }

            throw new SystemLookupItemAlreadyExistsException(model.Name);
        }

        public async Task<SystemLookupItemModel> Update(SystemLookupItemModel model)
        {
            var entity = await _systemLookupItemManager.GetItemAsync(model.Id);
            if (entity == null) throw new SystemLookupItemNotFoundException();

            if (entity != null)
            {
                entity.Values.ToList().ForEach(x => x.Id = x.Id != null ? x.Id : Guid.NewGuid().ToString());
                entity = await _systemLookupItemManager.UpsertGroupAsync(entity);
                model = new SystemLookupItemModel(entity);
                return model;
            }

            return null;
        }
    }
}
