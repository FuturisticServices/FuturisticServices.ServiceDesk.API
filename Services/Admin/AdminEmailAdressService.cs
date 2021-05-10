using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface IAdminEmailAddressService
    {
        Task<AdminEmailAddressModel> Validate(AdminEmailAddressModel model);
        Task<List<AdminEmailAddressModel>> Validate(List<AdminEmailAddressModel> model);
    }

    public class AdminEmailAddressService : AdminBaseService, IAdminEmailAddressService
    {
        private readonly IAdminLookupItemsService _adminLookupItemsService;
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminEmailAddressService(IAdminLookupItemsService adminLookupItemsService, IHashingService hashingService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _adminLookupItemsService = adminLookupItemsService;
            _hashingService = hashingService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task<AdminEmailAddressModel> Validate(AdminEmailAddressModel model)
        {
            if (model.Address == string.Empty) throw new AddressLine1IsRequiredException();

            model.Type = await _adminLookupItemsService.GetItem("Email Address Types", model.Type.Id);

            return model;
        }

        public async Task<List<AdminEmailAddressModel>> Validate(List<AdminEmailAddressModel> model)
        {
            foreach (AdminEmailAddressModel emailAddress in model)
            {
                if (string.IsNullOrEmpty(emailAddress.Address)) throw new EmailAddressIsRequiredException();

                emailAddress.Type = await _adminLookupItemsService.GetItem("Email Address Types", emailAddress.Type.Id);
            }

            return model;
        }
        #endregion Public methods
    }
}
