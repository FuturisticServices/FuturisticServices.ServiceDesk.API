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
    public interface IAdminPhoneNumberService
    {
        Task<AdminPhoneNumberModel> Validate(AdminPhoneNumberModel model);
        Task<List<AdminPhoneNumberModel>> Validate(List<AdminPhoneNumberModel> model);
    }

    public class AdminPhoneNumberService : AdminBaseService, IAdminPhoneNumberService
    {
        private readonly IAdminLookupItemsService _adminLookupItemsService;
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminPhoneNumberService(IAdminLookupItemsService adminLookupItemsService, IHashingService hashingService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _hashingService = hashingService;
            _adminLookupItemsService = adminLookupItemsService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task<AdminPhoneNumberModel> Validate(AdminPhoneNumberModel model)
        {
            if (string.IsNullOrEmpty(model.Number)) throw new PhoneNumberIsRequiredException();

            model.Type = await _adminLookupItemsService.GetItem("Phone Number Types", model.Type.Id);

            return model;
        }

        public async Task<List<AdminPhoneNumberModel>> Validate(List<AdminPhoneNumberModel> model)
        {
            foreach (AdminPhoneNumberModel phoneNumber in model)
            {
                if (string.IsNullOrEmpty(phoneNumber.Number)) throw new PhoneNumberIsRequiredException();

                phoneNumber.Type = await _adminLookupItemsService.GetItem("Phone Number Types", phoneNumber.Type.Id);
            }

            return model;
        }
        #endregion Public methods
    }
}
