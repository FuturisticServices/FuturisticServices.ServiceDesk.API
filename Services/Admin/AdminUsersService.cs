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

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Extensions;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface IAdminUsersService
    {
        Task CreateItem(AdminAuthenticateUserModel model);
        Task<AdminAuthenticateUser> GetItem(string username);
        Task<AdminAuthenticateUser> GetItem(Guid id);
        Task<IEnumerable<AdminUser>> GetItems();
        Task<string> GetUsernameAsync(string basicAuthHeader);
        Task<string> GetPasswordAsync(string basicAuthHeader);
        Task<AdminAuthenticateUserModel> AuthenticateAsync(string basicAuthHeader);
        Task<string> GenerateJwtToken(AdminAuthenticateUser user);
        Task<AdminUserModel> UpdateItem(AdminUserModel model);
        Task ResetUsername(AdminUserResetUsernameModel model);
        Task ResetPassword(AdminUserResetPasswordModel model);
        Task DeleteItem(Guid id);
    }

    public class AdminUsersService : AdminBaseService, IAdminUsersService
    {
        private readonly IAdminPhoneNumberService _adminPhoneNumberService;
        private readonly IAdminEmailAddressService _adminEmailAddressService;
        private readonly IAdminUsersManager _adminUsersManager;
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminUsersService(IAdminPhoneNumberService adminPhoneNumberService, IAdminEmailAddressService adminEmailAddressService, IAdminUsersManager adminUsersManager, IHashingService hashingService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _adminPhoneNumberService = adminPhoneNumberService;
            _adminEmailAddressService = adminEmailAddressService;
            _hashingService = hashingService;
            _adminUsersManager = adminUsersManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task CreateItem(AdminAuthenticateUserModel model)
        {
            model = await Validate(model);

            var adminUser = new AdminAuthenticateUser(model);
            adminUser.Password = _hashingService.EncryptString(model.Password);
            await _adminUsersManager.CreateItemAsync(adminUser);
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

        public async Task<AdminAuthenticateUserModel> AuthenticateAsync(string basicAuthHeader)
        {
            if (!basicAuthHeader.ToString().StartsWith("Basic")) throw new Exception("'Basic' header not found.");

            var loginUsername = await GetUsernameAsync(basicAuthHeader);
            var loginPassword = await GetPasswordAsync(basicAuthHeader);

            if (string.IsNullOrEmpty(loginUsername) || string.IsNullOrEmpty(loginPassword)) throw new LoginFailedException();

            var user = await _adminUsersManager.GetItemAsync(loginUsername);
            if (user == null) throw new UserNotFoundException();

            var password = _hashingService.DecryptString(user.Password);
            if (loginPassword == password) return new AdminAuthenticateUserModel(user);

            return null;
        }

        public async Task<AdminAuthenticateUser> GetItem(string username)
        {
            var results = await _adminUsersManager.GetItemAsync(username);
            return results;
        }

        public async Task<AdminAuthenticateUser> GetItem(Guid id)
        {
            var results = await _adminUsersManager.GetItemAsync(id);
            if (results == null) throw new UserNotFoundException();

            return results;
        }

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

        public async Task<AdminUserModel> UpdateItem(AdminUserModel model)
        {
            var adminAuthenticateUser = await GetItem(new Guid(model.Id));
            if (adminAuthenticateUser == null) throw new UserNotFoundException();

            adminAuthenticateUser = new AdminAuthenticateUser(model, adminAuthenticateUser);
            await _adminUsersManager.UpsertItemAsync(adminAuthenticateUser);
            return new AdminUserModel(adminAuthenticateUser);
        }

        public async Task ResetUsername(AdminUserResetUsernameModel model)
        {
            var adminUser = await GetItem(new Guid(model.Id));
            if (adminUser == null) throw new UserNotFoundException();

            if (string.Compare(_hashingService.DecryptString(adminUser.Password), model.ConfirmPassword, false) != 0) throw new PasswordsDoNotMatchException();
            if (await UsernameAlreadyExists(model.ResetUsername)) throw new UsernameAlreadyExistsException(model.ResetUsername);

            //  TODO: All or nothing!
            //  NoSQL databases do not support updating the partition key value of an existing item. Need to delete entire document and create new.
            //  Transactions using the same partition key (but this process uses 2 different partition keys) ~ https://devblogs.microsoft.com/cosmosdb/introducing-transactionalbatch-in-the-net-sdk/
            await _adminUsersManager.DeleteItemAsync(adminUser);

            adminUser.Username = model.ResetUsername;

            var adminAuthenticateUser = new AdminAuthenticateUserModel(adminUser);
            await CreateItem(adminAuthenticateUser);
        }

        public async Task ResetPassword(AdminUserResetPasswordModel model)
        {
            var adminUser = await GetItem(new Guid(model.Id));
            if (adminUser == null) throw new UserNotFoundException();

            if (string.Compare(_hashingService.DecryptString(adminUser.Password), model.ConfirmPassword, false) != 0) throw new PasswordsDoNotMatchException();

            adminUser.Password = _hashingService.EncryptString(model.ResetPassword);
            adminUser = await _adminUsersManager.UpsertItemAsync(adminUser);
        }

        public async Task DeleteItem(Guid id)
        {
            var systemUser = await GetItem(id);
            if (systemUser == null) throw new UserNotFoundException();

            systemUser.Enabled = false;
            await _adminUsersManager.UpsertItemAsync(systemUser);
        }
        #endregion Public methods

        #region Private methods
        private async Task<bool> UsernameAlreadyExists(string username)
        {
            var systemUser = await GetItem(username);
            return systemUser != null;
        }

        private async Task<AdminAuthenticateUserModel> Validate(AdminAuthenticateUserModel model)
        {
            if (await UsernameAlreadyExists(model.Username)) throw new UsernameAlreadyExistsException(model.Username);

            model.NameFirst = Helpers.ToTitleCase(model.NameFirst);
            model.NameLast = Helpers.ToTitleCase(model.NameLast);

            model.EmailAddresses = await _adminEmailAddressService.Validate(model.EmailAddresses);
            model.PhoneNumbers = await _adminPhoneNumberService.Validate(model.PhoneNumbers);

            return model;
        }
        #endregion Private methods
    }
}
