using System;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface IAddressService
    {
        Task<AddressModel> Validate(AddressModel model);
    }

    public class AddressService : SystemBaseService, IAddressService
    {
        private readonly ISystemLookupItemsService _systemLookupItemService;
        private readonly IHashingService _hashingService;
        private readonly ISystemUsersManager _systemUsersManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AddressService(ISystemLookupItemsService systemLookupItemService, IHashingService hashingService, ISystemUsersManager systemUsersManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemLookupItemService = systemLookupItemService;
            _hashingService = hashingService;
            _systemUsersManager = systemUsersManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task<AddressModel> Validate(AddressModel model)
        {
            if (model.Type.Id == string.Empty) throw new AddressTypeIdIsRequiredException();
            model.Type = await _systemLookupItemService.GetItem("Address Types", model.Type.Id);

            if (model.Line1 == string.Empty) throw new AddressLine1IsRequiredException();
            if (model.City == string.Empty) throw new AddressCityIsRequiredException();

            if (model.State.Id == string.Empty) throw new AddressStateIdIsRequiredException();
            model.State = await _systemLookupItemService.GetItem("States", model.State.Id);

            if (model.PostalCode == string.Empty) throw new AddressPostalCodeIsRequiredException();
            var code = model.PostalCode.Split('_');
            if (code.Length - 1 >= 0 && code[0] != null && !int.TryParse(code[0], out int code1)) throw new AddressPostalCodeIsInvalidException();
            if (code.Length - 1 >= 1 && code[1] != null && !int.TryParse(code[1], out int code2)) throw new AddressPostalCodeIsInvalidException();

            if (model.Country.Id == string.Empty) throw new AddressCountryIdIsRequiredException();
            model.Country = await _systemLookupItemService.GetItem("Countries", model.Country.Id);

            return model;
        }
        #endregion Public methods
    }
}
