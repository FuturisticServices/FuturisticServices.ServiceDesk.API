using System;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Extensions;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface IBillingInformationService
    {
        Task<BillingInformationModel> Validate(BillingInformationModel model);
    }

    public class BillingInformationService : SystemBaseService, IBillingInformationService
    {
        private readonly IAddressesService _addressService;
        private readonly ISystemLookupItemsService _systemLookupItemService;
        private readonly IHashingService _hashingService;
        private readonly ISystemUsersManager _systemUsersManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BillingInformationService(IAddressesService addressService, ISystemLookupItemsService systemLookupItemService, IHashingService hashingService, ISystemUsersManager systemUsersManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _addressService = addressService;
            _systemLookupItemService = systemLookupItemService;
            _hashingService = hashingService;
            _systemUsersManager = systemUsersManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task<BillingInformationModel> Validate(BillingInformationModel model)
        {
            if (model.NameOnCard == null || model.NameOnCard == string.Empty) throw new BillingInformationNameOnCardIsRequiredException();
            if (model.CardNumber == null || model.NameOnCard == string.Empty) throw new BillingInformationNameOnCardIsRequiredException();
            if (!Helpers.IsCreditCardNumberValid(model.CardNumber)) throw new BillingInformationCardNumberIsInvalidException();
            if (model.CardSecurityValue == null || model.CardSecurityValue == string.Empty) throw new BillingInformationCardSecurityNumberIsRequiredException();
            if (!int.TryParse(model.CardSecurityValue, out int csv)) throw new BillingInformationCardSecurityNumberIsInvalidException();
            model.Address = await _addressService.Validate(model.Address);

            return model;
        }
        #endregion Public methods
    }
}
