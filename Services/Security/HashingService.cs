using Microsoft.AspNetCore.DataProtection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace TangledServices.ServicePortal.API.Services
{
    //  https://mikaelkoskinen.net/post/encrypt-decrypt-string-asp-net-core

    public interface IHashingService
    {
        string EncryptString(string text);
        string DecryptString(string cipherText);
    }

    public class HashingService : IHashingService
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HashingService(IDataProtectionProvider dataProtectionProvider, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _dataProtectionProvider = dataProtectionProvider;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        /// <summary>
        /// https://www.infoworld.com/article/3431139/how-to-use-the-data-protection-api-in-aspnet-core.html
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string EncryptString(string text)
        {
            var key = _configuration.GetSection("hash:secretKey").Value; //  from appsettings.json
            return _dataProtectionProvider.CreateProtector(key).Protect(text);
        }

        /// <summary>
        /// https://www.infoworld.com/article/3431139/how-to-use-the-data-protection-api-in-aspnet-core.html
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string DecryptString(string text)
        {
            var key = _configuration.GetSection("hash:secretKey").Value; //  from appsettings.json
            return _dataProtectionProvider.CreateProtector(key).Unprotect(text);
        }
        #endregion Public methods
    }
}
