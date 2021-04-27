using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Extensions;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ISystemUsersService
    {
        Task<IEnumerable<SystemUser>> GetItems();
        Task<string> GetUsernameAsync(string basicAuthHeader);
        Task<string> GetPasswordAsync(string basicAuthHeader);
        Task<SystemUser> AuthenticateAsync(string basicAuthHeader);
        Task<string> GenerateJwtToken(SystemUser user);
        Task<string> GetUniqueEmployeeId(string moniker);
    }

    public class SystemUsersService : SystemBaseService, ISystemUsersService
    {
        private readonly IHashingService _hashingService;
        private readonly ISystemUsersManager _systemUsersManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemUsersService(IHashingService hashingService, ISystemUsersManager systemUsersManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _hashingService = hashingService;
            _systemUsersManager = systemUsersManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public Task<IEnumerable<SystemUser>> GetItems()
        {
            return _systemUsersManager.GetItemsAsync();
        }

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

        public async Task<SystemUser> AuthenticateAsync(string basicAuthHeader)
        {
            if (basicAuthHeader.ToString().StartsWith("Basic"))
            {
                var loginUsername = await GetUsernameAsync(basicAuthHeader);
                var loginPassword = await GetPasswordAsync(basicAuthHeader);

                if (!string.IsNullOrEmpty(loginUsername) && !string.IsNullOrEmpty(loginPassword))
                {
                    var user = await _systemUsersManager.GetItemAsync(loginUsername);
                    if (user != null)
                    {
                        var password = _hashingService.DecryptString(user.Password);
                        if (loginPassword == password) return user;
                    }
                }
            }

            return null;
        }

        public async Task<string> GetUniqueEmployeeId(string moniker)
        {
            var users = await _systemUsersManager.GetItemsAsync();
            string employeeId = string.Empty;
            bool employeeIdNotUnique = true;

            do
            {
                string randomNumber = Helpers.GetRandomNumber();
                employeeId = string.Format("{0}{1}", moniker, randomNumber);
                employeeIdNotUnique = users.Any(x => x.EmployeeId.ToLower() == employeeId.ToLower());

            } while (employeeIdNotUnique);

            return employeeId;
        }
        #endregion Public methods

        #region Private methods
        public async Task<string> GenerateJwtToken(SystemUser user)
        {
            string jwtSecretKey = _configuration.GetSection("jwt:secretKey").Value; //  from appsettings.json

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (string role in user.Roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: "tangled.services",
                audience: "tangled.services",
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: credentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }
        #endregion Private methods
    }
}
