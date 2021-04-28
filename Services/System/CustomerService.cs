using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

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
        private readonly ISystemService _systemService;
        private readonly IAdminLookupItemsService _adminLookupItemsService;
        private readonly IAdminUsersService _adminUsersService;
        private readonly IAddressesService _addressesService;
        private readonly IPhoneNumbersService _phoneNumbersService;
        private readonly IWebsitesService _websitesService;
        private readonly IPointOfContactService _pointOfContactService;
        private readonly ISystemLookupItemsService _systemLookupItemsService;
        private readonly ISystemUsersService _systemUsersService;
        private readonly ICustomersManager _customersManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public CustomerService(IAdminService adminService, ISystemService systemService, IAdminLookupItemsService adminLookupItemsService, IAdminUsersService adminUsersService, IAddressesService addressesService, IPhoneNumbersService phoneNumbersService, IWebsitesService websitesService, IPointOfContactService pointOfContactService, ISystemLookupItemsService systemLookupItemsService, ISystemUsersService systemUsersService, ICustomersManager customersManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemService = systemService;
            _adminService = adminService;
            _adminLookupItemsService = adminLookupItemsService;
            _adminUsersService = adminUsersService;
            _addressesService = addressesService;
            _phoneNumbersService = phoneNumbersService;
            _websitesService = websitesService;
            _pointOfContactService = pointOfContactService;
            _systemLookupItemsService = systemLookupItemsService;
            _systemUsersService = systemUsersService;
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

            //  Create Customer entity object.
            CustomerEntity customerEntity = new CustomerEntity(model);

            //  Persist customer to system database.
            customerEntity = await _customersManager.CreateItemAsync(customerEntity);

            //  Create customer database.
            DatabaseResponse databaseResponse = await _adminService.CreateDatabase();
            if (databaseResponse.StatusCode != HttpStatusCode.Created) throw new SystemDatabaseNotCreatedException();

            //  Clone 'LookupItems' container and clone items from system database.
            var containerResponse = await _adminService.CreateContainer("LookupItems", "/name");
            if (containerResponse.StatusCode != HttpStatusCode.Created) throw new AdminContainerNotCreatedException("LookupItems");

            var systemLookupItems = await _systemLookupItemsService.GetItems();
            foreach (LookupItem systemLookupItem in systemLookupItems)
            {
                if (systemLookupItem.CloneToAdminDatabase)
                {
                    LookupItemModel lookupItemModel = new LookupItemModel(systemLookupItem);
                    await _adminLookupItemsService.CreateItem(lookupItemModel);
                }
            }

            //  Create 'Users' container and clone items from system database.            
            containerResponse = await _adminService.CreateContainer("Users", "/employeeId");
            if (containerResponse.StatusCode != HttpStatusCode.Created) throw new AdminContainerNotCreatedException("Users");

            var systemUsers = await _systemUsersService.GetItems();
            foreach (SystemUser systemUser in systemUsers)
            {
                if (systemUser.CloneToAdminDatabase)
                {
                    AdminUserModel adminUserModel = new AdminUserModel(systemUser);
                    await _adminUsersService.CreateItem(adminUserModel);
                }
            }

            //  Create container 'Tenants' container.
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

            model.Address = await _addressesService.Validate(model.Address);
            model.PhoneNumber = await _phoneNumbersService.Validate(model.PhoneNumber);
            model.Website = await _websitesService.Validate(model.Website);
            model.PointOfContact = await _pointOfContactService.Validate(model.PointOfContact);

            return model;
        }
        #endregion Private methods
    }
}
