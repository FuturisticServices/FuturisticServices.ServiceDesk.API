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
        Task<LookupItemModel> GetItem(Enums.LookupItems item);
        Task<LookupItemModel> GetItem(string name);
        Task<LookupItemValueModel> GetItem(string name, string id);
        Task<IEnumerable<LookupItemModel>> GetItems();
        Task<LookupItemModel> CreateItem(LookupItemModel model);
        Task<LookupItemModel> Update(LookupItemModel model);
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

        public async Task<LookupItemModel> GetItem(Enums.LookupItems item)
        {
            var entity = await _systemLookupItemManager.GetItemAsync(item);
            var model = new LookupItemModel(entity);
            return model;
        }

        public async Task<LookupItemModel> GetItem(string name)
        {
            var entity = await _systemLookupItemManager.GetItemAsync(name);
            return entity == null ? null : new LookupItemModel(entity);
        }

        public async Task<LookupItemValueModel> GetItem(string name, string id)
        {
            LookupItemValue entity = await _systemLookupItemManager.GetItemAsync(name, id);
            var model = new LookupItemValueModel(entity);
            return model;
        }

        public async Task<IEnumerable<LookupItemModel>> GetItems()
        {
            var entitites = await _systemLookupItemManager.GetItemsAsync();
            var model = LookupItemModel.Construct(entitites);
            return model;
        }

        public async Task<LookupItemModel> CreateItem(LookupItemModel model)
        {
            if (await GetItem(model.Name) == null)
            {
                LookupItem entity = new LookupItem(model);
                entity = await _systemLookupItemManager.CreateItemAsync(entity);
                model = new LookupItemModel(entity);
                return model;
            }

            throw new SystemLookupItemAlreadyExistsException(model.Name);
        }

        public async Task<LookupItemModel> Update(LookupItemModel model)
        {
            var entity = await _systemLookupItemManager.GetItemAsync(model.Id);
            if (entity == null) throw new SystemLookupItemNotFoundException();

            if (entity != null)
            {
                entity.Values.ToList().ForEach(x => x.Id = x.Id != null ? x.Id : Guid.NewGuid().ToString());
                entity = await _systemLookupItemManager.UpsertGroupAsync(entity);
                model = new LookupItemModel(entity);
                return model;
            }

            return null;
        }
    }
}
