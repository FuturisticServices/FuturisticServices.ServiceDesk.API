using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServiceDesk.API.Common;
using TangledServices.ServiceDesk.API.Entities;
using TangledServices.ServiceDesk.API.Managers;
using TangledServices.ServiceDesk.API.Models;

namespace TangledServices.ServiceDesk.API.Services
{
    public interface ISystemLookupItemService
    {
        Task<LookupGroupEntity> GetItem(string groupName);
        Task<LookupItemEntity> GetItem(string groupName, string id);
        Task<IEnumerable<LookupGroupEntity>> GetItems();
        Task<LookupGroupEntity> CreateItem(LookupGroupPostModel model);
        Task<LookupGroupEntity> UpdateGroup(LookupGroupEntity model);
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

        public async Task<LookupGroupEntity> GetItem(string groupName)
        {
            LookupGroupEntity results = await _systemLookupItemManager.GetItemAsync(groupName);
            return results;
        }

        public async Task<LookupItemEntity> GetItem(string groupName, string id)
        {
            LookupItemEntity results = await _systemLookupItemManager.GetItemAsync(groupName, id);
            return results;
        }

        public async Task<IEnumerable<LookupGroupEntity>> GetItems()
        {
            var results = await _systemLookupItemManager.GetItemsAsync();
            return results;
        }

        public async Task<LookupGroupEntity> CreateItem(LookupGroupPostModel model)
        {
            if (await GetItem(model.Group) == null)
            {
                LookupGroupEntity lookupGroupEntity = new LookupGroupEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    Group = model.Group,
                    Label = model.Label,
                    Items = await ConvertModelToEntity(model.Items.ToList())
                };

                LookupGroupEntity results = await _systemLookupItemManager.CreateItemAsync(lookupGroupEntity);
                return results;
            }
            return null;
        }

        public async Task<LookupGroupEntity> UpdateGroup(LookupGroupEntity model)
        {
            var group = await _systemLookupItemManager.GetItemAsync(model.Group);

            if (group != null)
            {
                group.Items.ToList().ForEach(x => x.Id = x.Id != null ? x.Id : Guid.NewGuid().ToString());
                LookupGroupEntity results = await _systemLookupItemManager.UpsertGroupAsync(group);
                return results;
            }
            return null;
        }

        #region Private methods
        private async Task<IEnumerable<LookupItemEntity>> ConvertModelToEntity(List<LookupItemPostModel> model)
        {
            List<LookupItemEntity> lookupItemsEntity = new List<LookupItemEntity>();

            foreach (LookupItemPostModel lookupItemPostModel in model)
            {
                LookupItemEntity lookupItemEntity = new LookupItemEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = lookupItemPostModel.Name,
                    Abbreviation = lookupItemPostModel.Abbreviation,
                    Items = lookupItemPostModel.Items == null ? null : await ConvertModelToEntity(lookupItemPostModel.Items.ToList())
                };

                lookupItemsEntity.Add(lookupItemEntity);
            }

            return lookupItemsEntity;
        }
        #endregion Private methods
    }
}
