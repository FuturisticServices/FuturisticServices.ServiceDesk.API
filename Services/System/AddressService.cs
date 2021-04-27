using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface IEmailAddressService
    {
        Task<EmailAddressModel> Validate(EmailAddressModel model);
    }

    public class EmailAddressService : SystemBaseService, IEmailAddressService
    {
        private readonly IHashingService _hashingService;
        private readonly ISystemUsersManager _systemUsersManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailAddressService(IHashingService hashingService, ISystemUsersManager systemUsersManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _hashingService = hashingService;
            _systemUsersManager = systemUsersManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task<EmailAddressModel> Validate(EmailAddressModel model)
        {
            if (model.Address == string.Empty) throw new AddressLine1IsRequiredException();

            return model;
        }
        #endregion Public methods
    }
}
