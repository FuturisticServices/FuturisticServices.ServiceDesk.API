using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json.Linq;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Extensions;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ICustomerService
    {
        Task<bool> NameExists(string name);
        Task<bool> MonikerExists(string moniker);
        Task<bool> NotFound(Guid id);
        Task<bool> NotFound(string moniker);
        Task<bool> Active(string moniker);
        Task<bool> NotActive(string moniker);
        Task<IEnumerable<CustomerEntity>> Get();
        Task<CustomerEntity> Get(Guid id);
        Task<CustomerEntity> Get(string moniker);
        Task<CustomerModel> Create(CustomerModel model);
        Task<CustomerModel> Update(CustomerModel model);
        Task<CustomerModel> Delete(IdModel model);
    }

    public class CustomerService : SystemBaseService, ICustomerService
    {
        #region Members
        private readonly IAdminService _adminService;
        private readonly IAdminLookupItemsService _adminLookupItemsService;
        private readonly IAdminUsersService _adminUsersService;
        private readonly IAdminEmailAddressService _adminEmailAddressService;
        private readonly IAdminPhoneNumberService _adminPhoneNumberService;
        private readonly ISystemService _systemService;        
        private readonly ISystemAddressService _systemAddressService;
        private readonly ISystemPhoneNumberService _systemPhoneNumberService;
        private readonly ISystemWebsiteService _systemWebsiteService;
        private readonly ISystemPointOfContactService _systemPointOfContactService;
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly ISystemUserService _systemUserService;
        private readonly ICustomersManager _customersManager;
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public CustomerService(IAdminService adminService, ISystemService systemService, IAdminLookupItemsService adminLookupItemsService, IAdminEmailAddressService adminEmailAddressService, IAdminPhoneNumberService adminPhoneNumberService, IAdminUsersService adminUsersService, ISystemAddressService systemAddressService, ISystemPhoneNumberService systemPhoneNumberService, ISystemWebsiteService systemWebsiteService, ISystemPointOfContactService systemPointOfContactService, ISystemLookupItemService systemLookupItemService, ISystemUserService systemUserService, ICustomersManager customersManager, IHashingService hashingService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _hashingService = hashingService;
            _systemService = systemService;
            _adminService = adminService;
            _adminLookupItemsService = adminLookupItemsService;
            _adminEmailAddressService = adminEmailAddressService;
            _adminPhoneNumberService = adminPhoneNumberService;
            _adminUsersService = adminUsersService;
            _systemAddressService = systemAddressService;
            _systemPhoneNumberService = systemPhoneNumberService;
            _systemWebsiteService = systemWebsiteService;
            _systemPointOfContactService = systemPointOfContactService;
            _systemLookupItemService = systemLookupItemService;
            _systemUserService = systemUserService;
            _customersManager = customersManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region Public methods
        public async Task<bool> NameExists(string name)
        {
            CustomerEntity entity = await _customersManager.GetItemAsync(name);
            return entity != null;
        }

        public async Task<bool> MonikerExists(string moniker)
        {
            CustomerEntity entity = await _customersManager.GetItemByAdminMonikerAsync(moniker);
            return entity != null;
        }

        public async Task<bool> NotFound(Guid id)
        {
            CustomerEntity entity = await _customersManager.GetItemAsync(id);
            return entity == null;
        }

        public async Task<bool> NotFound(string name)
        {
            CustomerEntity entity = await _customersManager.GetItemAsync(name);
            return entity == null;
        }

        public async Task<bool> Active(string id)
        {
            CustomerEntity entity = await _customersManager.GetItemAsync(new Guid(id));
            return entity.Enabled;
        }

        public async Task<bool> NotActive(string name)
        {
            CustomerEntity entity = await _customersManager.GetItemAsync(name);
            return !entity.Enabled;
        }

        public async Task<IEnumerable<CustomerEntity>> Get()
        {
            IEnumerable<CustomerEntity> results = await _customersManager.GetItemsAsync();

            return results;
        }

        public async Task<CustomerEntity> Get(Guid id)
        {
            CustomerEntity entity = await _customersManager.GetItemAsync(id);

            if (entity == null) throw new CustomerNotFoundException();

            return entity;
        }

        public async Task<CustomerEntity> Get(string adminMoniker)
        {
            CustomerEntity entity = await _customersManager.GetItemAsync(adminMoniker);

            if (entity == null) throw new CustomerNotFoundException(adminMoniker);

            return entity;
        }

        public async Task<CustomerModel> Create(CustomerModel model)
        {
            model = await Validate(model);

            model.AdminMoniker = model.AdminMoniker.ToUpper();
            model.TenantMonikers = new List<string>() { model.AdminMoniker };

            //  Create customer entity 
            CustomerEntity customerEntity = new CustomerEntity(model);

            //  Create customer item in system database.
            customerEntity = await _customersManager.CreateItemAsync(customerEntity);

            //  Create customer admin database.
            DatabaseResponse databaseResponse = await _adminService.CreateDatabase();
            if (databaseResponse.StatusCode != HttpStatusCode.Created) throw new SystemDatabaseNotCreatedException();

            //  Clone 'LookupItems' container and clone items from system database.
            var containerResponse = await _adminService.CreateContainer("LookupItems", "/name");
            if (containerResponse.StatusCode != HttpStatusCode.Created) throw new AdminContainerNotCreatedException("LookupItems");

            var systemLookupItemModels = await _systemLookupItemService.GetItems();
            var systemLookupItems = SystemLookupItem.Construct(systemLookupItemModels);
            foreach (SystemLookupItem systemLookupItem in systemLookupItems)
            {
                if (systemLookupItem.CloneToAdminDatabase)
                {
                    var adminLookupItemModel = new AdminLookupItemModel(systemLookupItem);
                    await _adminLookupItemsService.CreateItem(adminLookupItemModel);
                }
            }

            //  Create 'Users' container and clone items from system database.            
            containerResponse = await _adminService.CreateContainer("Users", "/username");
            if (containerResponse.StatusCode != HttpStatusCode.Created) throw new AdminContainerNotCreatedException("Users");

            var systemAuthenticateUsers = await _systemUserService.GetItems();
            foreach (SystemAuthenticateUser systemAuthenticateUser in systemAuthenticateUsers)
            {
                if (systemAuthenticateUser.CloneToAdminDatabase)
                {
                    AdminAuthenticateUserModel adminAuthenticateUserModel = new AdminAuthenticateUserModel(systemAuthenticateUser);
                    await _adminUsersService.CreateItem(adminAuthenticateUserModel);
                }
            }

            //  Create customer 'admin' user.
            foreach (AdminAuthenticateUserModel adminUserModel in model.Users)
            {
                adminUserModel.NameFirst = adminUserModel.NameFirst.Replace("{moniker}", model.AdminMoniker.ToUpper());
                adminUserModel.Username = adminUserModel.Username.Replace("{moniker}", model.AdminMoniker.ToLower());
                adminUserModel.DisplayAs = adminUserModel.DisplayAs.Replace("{moniker}", model.AdminMoniker.ToUpper());
                adminUserModel.EmailAddresses.ForEach(x => x.Address = x.Address.Replace("{moniker}", model.AdminMoniker.ToLower()));
                //for (int i = 0; i < adminUserModel.Roles.Count; i++) adminUserModel.Roles[i] = adminUserModel.Roles[i].Replace("{moniker}", model.AdminMoniker.ToUpper());

                var randomWord = await Helpers.GetRandomWordFromWordsApi();
                var joPassword = JObject.Parse(randomWord)["word"];
                adminUserModel.Password = _hashingService.EncryptString(joPassword.ToString());
                adminUserModel.EmailAddresses = await _adminEmailAddressService.Validate(adminUserModel.EmailAddresses);
                adminUserModel.PhoneNumbers = await _adminPhoneNumberService.Validate(adminUserModel.PhoneNumbers);

                await _adminUsersService.CreateItem(adminUserModel);
            }

            //  Create Tenants container.
            containerResponse = await _adminService.CreateContainer("Tenants", "/moniker");
            if (containerResponse.StatusCode != HttpStatusCode.Created) throw new AdminContainerNotCreatedException("Tenants");

            model = new CustomerModel(customerEntity);

            return model;
        }

        public async Task<CustomerModel> Update(CustomerModel model)
        {
            model = await Validate(model);

            //  Get Customer entity to update.
            CustomerEntity entity = await Get(new Guid(model.Id));

            //  Update Customer entity object with modifications from model.
            entity = new CustomerEntity(model);

            //  Persist system tenant to DB.
            entity = await _customersManager.UpserItemAsync(entity);

            CustomerModel response = new CustomerModel(entity);

            return response;
        }

        public async Task<CustomerModel> Delete(IdModel model)
        {
            //  Get Customer entity to delete.
            CustomerEntity customer = await Get(new Guid(model.Id));

            //  Indicate Customer is no longer enabled.
            customer.Enabled = false;

            //  Persist system tenant to DB.
            customer = await _customersManager.UpserItemAsync(customer);

            CustomerModel response = new CustomerModel(customer);

            return response;
        }
        #endregion Public methods

        #region Private methods
        private async Task<CustomerModel> Validate(CustomerModel model)
        {
            if (model.Id == null)
            {
                if (await NameExists(model.Name)) throw new CustomerAlreadyExistsException();
            }
            else
            {
                if (await NotFound(new Guid(model.Id))) throw new CustomerNotFoundException();
            }

            if (model.Name == null || model.Name == string.Empty) throw new NameIsRequiredException();
            if (model.AdminMoniker == null || model.AdminMoniker == string.Empty) throw new MonikerIsRequiredException();
            if (await MonikerExists(model.AdminMoniker)) throw new MonikerAlreadyExistsException();

            model.Name = Helpers.ToTitleCase(model.Name);
            model.LegalName = model.Name;

            model.Address = await _systemAddressService.Validate(model.Address);
            model.PhoneNumber = await _systemPhoneNumberService.Validate(model.PhoneNumber);
            model.Website = await _systemWebsiteService.Validate(model.Website);
            model.PointOfContact = await _systemPointOfContactService.Validate(model.PointOfContact);

            return model;
        }
        #endregion Private methods
    }
}
