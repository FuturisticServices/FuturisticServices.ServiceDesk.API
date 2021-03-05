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
    public interface ITenantUserService
    {
        Task<Entities.User> CreateItem(Entities.User user);
    }

    public class TenantUserService : TenantBaseService, ITenantUserService
    {
        #region Members
        private readonly ITenantUserManager _tenantUserManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public TenantUserService(ITenantUserManager tenantUserManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _tenantUserManager = tenantUserManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region Public methods
        public async Task<Entities.User> CreateItem(Entities.User user)
        {
            var results = await _tenantUserManager.CreateItemAsync(user);
            return results;
        }
        #endregion Public methods
    }
}