using System;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ICompanyService
    {
        Task<CustomerModel> Validate(CustomerModel model);
    }

    public class CompanyService : SystemBaseService, ICompanyService
    {
        private readonly ISystemAddressService _addressService;
        private readonly ISystemPhoneNumberService _phoheNumberService;
        private readonly ISystemWebsiteService _websiteService;
        private readonly IHashingService _hashingService;
        private readonly ISystemUserManager _systemUsersManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyService(ISystemAddressService addressService, ISystemPhoneNumberService phoneNumberService, ISystemWebsiteService websiteService, IHashingService hashingService, ISystemUserManager systemUsersManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _addressService = addressService;
            _phoheNumberService = phoneNumberService;
            _websiteService = websiteService;
            _hashingService = hashingService;
            _systemUsersManager = systemUsersManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task<CustomerModel> Validate(CustomerModel model)
        {
            if (model.Name == string.Empty) throw new CompanyNameIsRequiredException();

            await _addressService.Validate(model.Address);
            await _phoheNumberService.Validate(model.PhoneNumber);
            await _websiteService.Validate(model.Website);

            return model;
        }
        #endregion Public methods
    }
}
