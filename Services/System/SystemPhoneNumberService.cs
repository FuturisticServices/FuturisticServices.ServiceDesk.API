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
    public interface ISystemPhoneNumberService
    {
        Task<SystemPhoneNumberModel> Validate(SystemPhoneNumberModel model);
        Task<List<SystemPhoneNumberModel>> Validate(List<SystemPhoneNumberModel> model);
    }

    public class SystemPhoneNumberService : SystemBaseService, ISystemPhoneNumberService
    {
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemPhoneNumberService(ISystemLookupItemService systemLookupItemService, IHashingService hashingService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _hashingService = hashingService;
            _systemLookupItemService = systemLookupItemService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task<SystemPhoneNumberModel> Validate(SystemPhoneNumberModel model)
        {
            if (string.IsNullOrEmpty(model.Number)) throw new PhoneNumberIsRequiredException();

            model.Type = await _systemLookupItemService.GetItem("Phone Number Types", model.Type.Id);

            return model;
        }

        public async Task<List<SystemPhoneNumberModel>> Validate(List<SystemPhoneNumberModel> model)
        {
            foreach (SystemPhoneNumberModel phoneNumber in model)
            {
                if (string.IsNullOrEmpty(phoneNumber.Number)) throw new PhoneNumberIsRequiredException();

                phoneNumber.Type = await _systemLookupItemService.GetItem("Phone Number Types", phoneNumber.Type.Id);
            }

            return model;
        }
        #endregion Public methods
    }
}
