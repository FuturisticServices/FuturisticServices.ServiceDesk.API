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
    public interface ITenantRegistrationService
    {
    }

    public class TenantRegistrationService : TenantBaseService, ITenantRegistrationService
    {
        #region Members
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly ISystemSubscriptionService _systemSubscriptionService;
        private readonly ISystemTenantService _systemTenantService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Public methods
        public TenantRegistrationService(ISystemLookupItemService systemLookupItemService, ISystemSubscriptionService systemSubscriptionService, ISystemTenantService systemTenantService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemLookupItemService = systemLookupItemService;
            _systemSubscriptionService = systemSubscriptionService;
            _systemTenantService = systemTenantService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Public methods
    }
}