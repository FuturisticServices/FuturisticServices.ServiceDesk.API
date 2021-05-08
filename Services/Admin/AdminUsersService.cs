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
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface IAdminUsersService
    {
        Task<AdminAuthenticateUser> CreateItem(AdminAuthenticateUserModel model);
        Task<IEnumerable<AdminUser>> GetItems();
        Task<string> GetUsernameAsync(string basicAuthHeader);
        Task<string> GetPasswordAsync(string basicAuthHeader);
        Task<AdminUser> AuthenticateAsync(string basicAuthHeader);
        Task<string> GenerateJwtToken(AdminAuthenticateUser user);
    }

    public class AdminUsersService : AdminBaseService, IAdminUsersService
    {
        private readonly IHashingService _hashingService;
        private readonly IAdminUsersManager _adminUsersManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminUsersService(IHashingService hashingService, IAdminUsersManager adminUsersManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _hashingService = hashingService;
            _adminUsersManager = adminUsersManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public Task<AdminAuthenticateUser> CreateItem(AdminAuthenticateUserModel model)
        {
            AdminAuthenticateUser entity = new AdminAuthenticateUser(model);
            return _adminUsersManager.CreateItemAsync(entity);
        }

        public Task<IEnumerable<AdminUser>> GetItems()
        {
            return _adminUsersManager.GetItemsAsync();
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

        public async Task<AdminUser> AuthenticateAsync(string basicAuthHeader)
        {
            if (basicAuthHeader.ToString().StartsWith("Basic"))
            {
                var loginUsername = await GetUsernameAsync(basicAuthHeader);
                var loginPassword = await GetPasswordAsync(basicAuthHeader);

                if (!string.IsNullOrEmpty(loginUsername) && !string.IsNullOrEmpty(loginPassword))
                {
                    var user = await _adminUsersManager.GetItemAsync(loginUsername);
                    if (user != null)
                    {
                        var password = _hashingService.DecryptString(user.Password);
                        if (loginPassword == password) return user;
                    }
                }
            }

            return null;
        }
        #endregion Public methods

        #region Private methods
        public async Task<string> GenerateJwtToken(AdminAuthenticateUser user)
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
