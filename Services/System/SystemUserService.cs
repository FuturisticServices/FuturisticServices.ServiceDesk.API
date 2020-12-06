using System;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using FuturisticServices.ServiceDesk.API.Managers;

namespace FuturisticServices.ServiceDesk.API.Services
{
    public interface ISystemUserService
    {
        Task<string> GetUsernameAsync(string basicAuthHeader);
        Task<string> GetPasswordAsync(string basicAuthHeader);
        Task<bool> AuthenticateAsync(string basicAuthHeader);
        Task<string> GenerateJwtToken(string username);
    }

    public class SystemUserService : SystemBaseService, ISystemUserService
    {
        private readonly IHashingService _hashingService;
        private readonly ISystemUserManager _systemUserManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemUserService(IHashingService hashingService, ISystemUserManager systemUserManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _hashingService = hashingService;
            _systemUserManager = systemUserManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task<string> GetUsernameAsync(string basicAuthHeader)
        {
            if (basicAuthHeader.ToString().StartsWith("Basic"))
            {
                var userCredentials = basicAuthHeader.ToString().Substring("Basic ".Length).Trim();
                var usernameAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(userCredentials)).Split(':');
                return usernameAndPassword[0];
            }

            return string.Empty;
        }

        public async Task<string> GetPasswordAsync(string basicAuthHeader)
        {
            if (basicAuthHeader.ToString().StartsWith("Basic"))
            {
                var userCredentials = basicAuthHeader.ToString().Substring("Basic ".Length).Trim();
                var usernameAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(userCredentials)).Split(':');
                return usernameAndPassword[1];
            }

            return string.Empty;
        }

        public async Task<bool> AuthenticateAsync(string basicAuthHeader)
        {
            if (basicAuthHeader.ToString().StartsWith("Basic"))
            {
                var username = await GetUsernameAsync(basicAuthHeader);
                var password = await GetPasswordAsync(basicAuthHeader);

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    var user = await _systemUserManager.GetItemAsync(username);
                    if (user != null)
                    {
                        return password == _hashingService.DecryptString(user.Password);
                    }
                }
            }

            return false;
        }
        #endregion Public methods

        #region Private methods
        public async Task<string> GenerateJwtToken(string username)
        {
            string jwtSecretKey = _configuration.GetSection("keys:jwtSecretKey").Value;

            var claimsData = new[] { new Claim(ClaimTypes.Name, username) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: "futuristic.service",
                audience: "futuristic.service",
                expires: DateTime.Now.AddDays(365),
                claims: claimsData,
                signingCredentials: credentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }
        #endregion Private methods
    }
}
