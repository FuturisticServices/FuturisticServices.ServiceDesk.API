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
    public interface IPhoneNumbersService
    {
        Task<SystemPhoneNumberModel> Validate(SystemPhoneNumberModel model);
    }

    public class PhoneNumbersService : SystemBaseService, IPhoneNumbersService
    {
        private readonly IHashingService _hashingService;
        private readonly ISystemUsersManager _systemUsersManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PhoneNumbersService(IHashingService hashingService, ISystemUsersManager systemUsersManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _hashingService = hashingService;
            _systemUsersManager = systemUsersManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task<SystemPhoneNumberModel> Validate(SystemPhoneNumberModel model)
        {
            if (model.Number == string.Empty) throw new PhoneNumberIsRequiredException();

            return model;
        }
        #endregion Public methods
    }
}
