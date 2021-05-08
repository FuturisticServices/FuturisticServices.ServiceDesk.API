using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Extensions;
using TangledServices.ServicePortal.API.Managers;
using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ISystemUsersService
    {
        Task CreateItem(SystemAuthenticateUser systemUser);
        Task<SystemUserModel> UpdateItem(SystemUserModel model);
        Task<IEnumerable<SystemUser>> GetItems();
        Task<SystemAuthenticateUser> GetItem(Guid id);
        Task<string> GetUsernameAsync(string basicAuthHeader);
        Task<string> GetPasswordAsync(string basicAuthHeader);
        Task<SystemUserAuthenticateModel> AuthenticateAsync(string basicAuthHeader);
        Task ResetUsername(SystemUserResetUsernameModel model);
        Task ResetPassword(SystemUserResetPasswordModel model);
        Task<string> GenerateJwtToken(SystemAuthenticateUser user);
        Task DeleteItem(Guid id);
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
        public async Task CreateItem(SystemAuthenticateUser systemUser)
        {
            await _systemUsersManager.CreateItemAsync(systemUser);
        }

        public async Task<SystemUserModel> UpdateItem(SystemUserModel model)
        {
            var systemAuthenticateUser = await GetItem(new Guid(model.Id));
            if (systemAuthenticateUser == null) throw new UserNotFoundException();

            systemAuthenticateUser = new SystemAuthenticateUser(model, systemAuthenticateUser);
            await _systemUsersManager.UpsertItemAsync(systemAuthenticateUser);
            return new SystemUserModel(systemAuthenticateUser);
        }

        public async Task<IEnumerable<SystemUser>> GetItems()
        {
            return await _systemUsersManager.GetItemsAsync();
        }

        public async Task<SystemAuthenticateUser> GetItem(string username)
        {
            var results = await _systemUsersManager.GetItemAsync(username);
            return results;
        }

        public async Task<SystemAuthenticateUser> GetItem(Guid id)
        {
            var results = await _systemUsersManager.GetItemAsync(id);
            return results;
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

        public async Task<SystemUserAuthenticateModel> AuthenticateAsync(string basicAuthHeader)
        {
            if (!basicAuthHeader.ToString().StartsWith("Basic")) throw new Exception("'Basic' header not found.");

            var loginUsername = await GetUsernameAsync(basicAuthHeader);
            var loginPassword = await GetPasswordAsync(basicAuthHeader);

            if (string.IsNullOrEmpty(loginUsername) || string.IsNullOrEmpty(loginPassword)) throw new LoginFailedException();

            var user = await _systemUsersManager.GetItemAsync(loginUsername);
            if (user == null) throw new UserNotFoundException();

            var password = _hashingService.DecryptString(user.Password);
            if (loginPassword == password) return new SystemUserAuthenticateModel(user);

            return null;
        }

        public async Task ResetUsername(SystemUserResetUsernameModel model)
        {
            var systemUser = await GetItem(new Guid(model.Id));
            if (systemUser == null) throw new UserNotFoundException();

            if (string.Compare(_hashingService.DecryptString(systemUser.Password), model.ConfirmPassword, false) != 0) throw new PasswordsDoNotMatchException();
            if (await UsernameAlreadyExists(model.ResetUsername)) throw new UsernameAlreadyExistsException(model.ResetUsername);

            //  TODO: All or nothing!
            //  NoSQL databases do not support updating the partition key value of an existing item. Need to delete entire document and create new.
            //  Transactions using the same partition key (but this process uses 2 different partition keys) ~ https://devblogs.microsoft.com/cosmosdb/introducing-transactionalbatch-in-the-net-sdk/
            await _systemUsersManager.DeleteItemAsync(systemUser);

            systemUser.Username = model.ResetUsername;
            await CreateItem(systemUser);
        }

        public async Task ResetPassword(SystemUserResetPasswordModel model)
        {
            var systemUser = await GetItem(new Guid(model.Id));
            if (systemUser == null) throw new UserNotFoundException();

            if (string.Compare(_hashingService.DecryptString(systemUser.Password), model.ConfirmPassword, false) != 0) throw new PasswordsDoNotMatchException();

            systemUser.Password = _hashingService.EncryptString(model.ResetPassword);
            systemUser = await _systemUsersManager.UpsertItemAsync(systemUser);
        }

        public async Task<string> GenerateJwtToken(SystemAuthenticateUser user)
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
                expires: DateTime.Now.AddDays(365),
                claims: authClaims,
                signingCredentials: credentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        public async Task DeleteItem(Guid id)
        {
            var systemUser = await GetItem(id);
            if (systemUser == null) throw new UserNotFoundException();

            systemUser.Enabled = false;
            await _systemUsersManager.UpsertItemAsync(systemUser);
        }
        #endregion Public methods

        #region Private methods
        private async Task<bool> UsernameAlreadyExists(string username)
        {
            var systemUser = await GetItem(username);
            return systemUser != null;
        }
        #endregion Private methods
    }
}
