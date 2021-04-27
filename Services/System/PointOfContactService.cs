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
    public interface IPointOfContactService
    {
        Task<PointOfContactModel> Validate(PointOfContactModel model);
    }

    public class PointOfContactService : SystemBaseService, IPointOfContactService
    {
        private readonly IAddressesService _addressService;
        private readonly IPhoneNumbersService _phoheNumberService;
        private readonly IEmailAddressService _emailAddressService;
        private readonly IHashingService _hashingService;
        private readonly ISystemUsersManager _systemUsersManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PointOfContactService(IAddressesService addressService, IPhoneNumbersService phoneNumberService, IEmailAddressService emailAddressService, IHashingService hashingService, ISystemUsersManager systemUsersManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _addressService = addressService;
            _phoheNumberService = phoneNumberService;
            _emailAddressService = emailAddressService;
            _hashingService = hashingService;
            _systemUsersManager = systemUsersManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task<PointOfContactModel> Validate(PointOfContactModel model)
        {
            if (model.FirstName == string.Empty) throw new PointOfContactFirstNameIsRequiredException();
            if (model.LastName == string.Empty) throw new PointOfContactLastNameIsRequiredException();
            if (model.Title == string.Empty) throw new PointOfContactTitleIsRequiredException();

            await _addressService.Validate(model.Address);
            await _phoheNumberService.Validate(model.PhoneNumber);
            await _emailAddressService.Validate(model.EmailAddress);

            return model;
        }
        #endregion Public methods
    }
}
