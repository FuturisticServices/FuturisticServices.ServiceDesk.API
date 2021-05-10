using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ISystemEmailAddressService
    {
        Task<SystemEmailAddressModel> Validate(SystemEmailAddressModel model);
        Task<List<SystemEmailAddressModel>> Validate(List<SystemEmailAddressModel> model);
    }

    public class SystemEmailAddressService : SystemBaseService, ISystemEmailAddressService
    {
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemEmailAddressService(ISystemLookupItemService systemLookupItemService, IHashingService hashingService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemLookupItemService = systemLookupItemService;
            _hashingService = hashingService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task<SystemEmailAddressModel> Validate(SystemEmailAddressModel model)
        {
            if (model.Address == string.Empty) throw new AddressLine1IsRequiredException();

            model.Type = await _systemLookupItemService.GetItem("Email Address Types", model.Type.Id);

            return model;
        }

        public async Task<List<SystemEmailAddressModel>> Validate(List<SystemEmailAddressModel> model)
        {
            foreach (SystemEmailAddressModel emailAddress in model)
            {
                if (string.IsNullOrEmpty(emailAddress.Address)) throw new EmailAddressIsRequiredException();

                emailAddress.Type = await _systemLookupItemService.GetItem("Email Address Types", emailAddress.Type.Id);
            }
            
            return model;
        }
        #endregion Public methods
    }
}
