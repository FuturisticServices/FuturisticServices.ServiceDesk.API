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
    public interface ITenantLookupItemService
    {
        Task<LookupItemEntity> GetItem(string groupName, string itemName);
        Task<LookupGroupEntity> CreateItem(LookupGroupEntity lookupGroup);
    }

    public class TenantLookupItemService : TenantBaseService, ITenantLookupItemService
    {
        private readonly ITenantLookupItemManager _tenantLookupItemManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TenantLookupItemService(ITenantLookupItemManager tenantLookupItemManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _tenantLookupItemManager = tenantLookupItemManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<LookupItemEntity> GetItem(string groupName, string itemName)
        {
            var results = await _tenantLookupItemManager.GetItemAsync(groupName, itemName);
            return results;
        }

        public async Task<LookupGroupEntity> CreateItem(LookupGroupEntity lookupGroup)
        {
            var results = await _tenantLookupItemManager.CreateItemAsync(lookupGroup);
            return results;
        }
    }
}