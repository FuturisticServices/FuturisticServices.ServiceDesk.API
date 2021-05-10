using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ISystemWebsiteService
    {
        Task<SystemWebsiteModel> Validate(SystemWebsiteModel model);
        Task<List<SystemWebsiteModel>> Validate(List<SystemWebsiteModel> model);
    }

    public class SystemWebsiteService : SystemBaseService, ISystemWebsiteService
    {
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemWebsiteService(ISystemLookupItemService systemLookupItemService, IHashingService hashingService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _hashingService = hashingService;
            _systemLookupItemService = systemLookupItemService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task<SystemWebsiteModel> Validate(SystemWebsiteModel model)
        {
            if (string.IsNullOrEmpty(model.Url)) throw new WebsiteUrlIsRequiredException();

            model.Type = await _systemLookupItemService.GetItem("Website Types", model.Type.Id);

            return model;
        }

        public async Task<List<SystemWebsiteModel>> Validate(List<SystemWebsiteModel> model)
        {
            foreach (SystemWebsiteModel website in model)
            {
                if (string.IsNullOrEmpty(website.Url)) throw new WebsiteUrlIsRequiredException();

                website.Type = await _systemLookupItemService.GetItem("Website Types", website.Type.Id);
            }

            return model;
        }
        #endregion Public methods
    }
}
