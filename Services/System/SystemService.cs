using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Managers;
using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Services
{
    /// <summary>
    /// Endpoint interfaces to control/manage the [TangledServices.ServicePortal] database.
    /// </summary>
    public interface ISystemService
    {
        Task Reset();
        Task<ContainerProperties> GetContainerProperties(string name);
        Task<IEnumerable<Container>> GetContainers();
        Container GetContainer(string name);
        bool ContainerExists(string name);
    }

    /// <summary>
    /// Endpoints to control/manage the [TangledServices.ServicePortal] database.
    /// </summary>
    public class SystemService : SystemBaseService, ISystemService
    {
        #region Members
        private readonly IHashingService _hashingService;
        private readonly ISystemUserService _systemUserService;
        private readonly ISystemEmailAddressService _systemEmailAddressService;
        private readonly ISystemPhoneNumberService _systemPhoneNumberService;
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly ISystemSubscriptionService _systemSubscriptionService;
        private readonly ISystemDepartmentService _systemDepartmentService;

        private readonly ISystemManager _systemManager;
        private readonly ISystemUserManager _systemUsersManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="systemUserService">System services.</param>
        /// <param name="hashingService">Hashing services.</param>
        /// <param name="systemEmailAddressService">System email address services.</param>
        /// <param name="systemPhoneNumberService">System phone number services.</param>
        /// <param name="systemLookupItemService">System lookup item services.</param>
        /// <param name="systemSubscriptionService">System subscription services.</param>
        /// <param name="systemDepartmentService">System department services.</param>
        /// <param name="systemUserManager">System database user managers.</param>
        /// <param name="systemManager">System database managers.</param>
        /// <param name="configuration">Microsoft configuration services.</param>
        /// <param name="webHostEnvironment">Microsoft web hosting environment services.</param>
        public SystemService(ISystemUserService systemUserService, 
            IHashingService hashingService, 
            ISystemEmailAddressService systemEmailAddressService,
            ISystemPhoneNumberService systemPhoneNumberService,
            ISystemLookupItemService systemLookupItemService, 
            ISystemSubscriptionService systemSubscriptionService, 
            ISystemDepartmentService systemDepartmentService,
            ISystemUserManager systemUserManager,
            ISystemManager systemManager,
            IConfiguration configuration, 
            IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemUserService = systemUserService;
            _hashingService = hashingService;
            _systemEmailAddressService = systemEmailAddressService;
            _systemPhoneNumberService = systemPhoneNumberService;
            _systemLookupItemService = systemLookupItemService;
            _systemSubscriptionService = systemSubscriptionService;
            _systemDepartmentService = systemDepartmentService;
            _systemUsersManager = systemUserManager;
            _systemManager = systemManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Resets the TangledServices.ServicePortal database by -
        ///     1.  If it exists, deletes the TangledServices.ServicePortal database.
        ///     2.  Creates the TangledServices.ServicePortal database.
        ///     3.  Creates the required containers.
        ///     4.  Persists data to the containers.
        /// </summary>
        /// <returns></returns>
        public async Task Reset()
        {
            var databaseResponse = await _systemManager.CreateDatabase();
            if (databaseResponse.StatusCode != HttpStatusCode.Created) throw new SystemDatabaseNotCreatedException();

            //  Create containers.
            var containers = _configuration.GetSection("system:reset:containers").Get<List<SystemResetModel>>(); //  from system-reset.json

            //  Persist data to containers.
            foreach (SystemResetModel container in containers)
            {
                var containerResponse = await createContainer(container);
                await createContainerLookupItems(container);
                await createContainerSubscriptions(container);
                await createContainerUsers(container);
                await createContainerDepartments(container);
            }
        }

        public async Task<ContainerProperties> GetContainerProperties(string name)
        {
            var response = await _systemManager.GetContainerProperties(name);
            return response;
        }

        /// <summary>
        /// Get a list of containers that exist in the System database.
        /// </summary>
        /// <returns>Returns a list of Container objects.</Container></returns>
        public async Task<IEnumerable<Container>> GetContainers()
        {
            var response = await _systemManager.GetContainers();
            return response;
        }

        /// <summary>
        /// Retrieves an existing container from the System database.
        /// </summary>
        /// <param name="name">Name of the container to retrieve.</param>
        /// <returns>A Container object.</returns>
        public Container GetContainer(string name)
        {
            Container response = _systemManager.GetContainer(name);
            return response;
        }

        /// <summary>
        /// Determines if a container already exists in the System database.
        /// </summary>
        /// <param name="name">Name of the container to retrieve.</param>
        /// <returns>True if the container exists. False otherwise.</returns>
        public bool ContainerExists(string name)
        {
            Container container = _systemManager.GetContainer(name);
            return container != null;
        }

        #region Private methods
        /// <summary>
        /// Creates a container in the System database.
        /// </summary>
        /// <param name="model">A SystemResetModel object.</param>
        /// <returns>ContainerResponse object.</returns>
        private async Task<ContainerResponse> createContainer(SystemResetModel model)
        {
            ContainerResponse containerResponse = await _systemManager.CreateContainer(model.Name, model.PartitionKeyPath);
            return containerResponse;
        }

        /// <summary>
        /// Creates multiple items in the System LookupItems container.
        /// </summary>
        /// <param name="model">A SystemResetModel object.</param>
        /// <returns></returns>
        private async Task createContainerLookupItems(SystemResetModel model)
        {
            if (model.LookupItems == null) return;

            foreach (SystemLookupItemModel systemLookupItemModel in model.LookupItems)
            {
                await _systemLookupItemService.CreateItem(systemLookupItemModel);
            }
        }

        /// <summary>
        /// Creates multiple items in the System Subscriptions container.
        /// </summary>
        /// <param name="model">A SystemResetModel object.</param>
        /// <returns></returns>
        private async Task createContainerSubscriptions(SystemResetModel model)
        {
            if (model.Subscriptions == null) return;

            foreach (SubscriptionModel subscriptionModel in model.Subscriptions)
            {
                //  If a renewal timeframe exists, retrieve it. Otherwise, make it null.
                subscriptionModel.RenewalTimeframe = subscriptionModel.RenewalTimeframe == null ? null : await _systemLookupItemService.GetItem("Subscription Renewal Timeframes", subscriptionModel.RenewalTimeframe.Id);
                var entity = new Subscription(subscriptionModel);
                await _systemSubscriptionService.CreateItem(entity);
            }
        }

        /// <summary>
        /// Creates multiple items in the System Users container.
        /// </summary>
        /// <param name="model">A SystemResetModel object.</param>
        /// <returns></returns>
        private async Task createContainerUsers(SystemResetModel model)
        {
            if (model.Users == null) return;

            foreach (SystemUserAuthenticateModel systemAuthenticationUserModel in model.Users)
            {
                //  Validate and populate lookup items.
                systemAuthenticationUserModel.EmailAddresses = await _systemEmailAddressService.Validate(systemAuthenticationUserModel.EmailAddresses);
                systemAuthenticationUserModel.PhoneNumbers = await _systemPhoneNumberService.Validate(systemAuthenticationUserModel.PhoneNumbers);

                //  Encrypt password.
                systemAuthenticationUserModel.Password = _hashingService.EncryptString(systemAuthenticationUserModel.Password);

                //  Persist item.
                await _systemUserService.CreateItem(systemAuthenticationUserModel);
            }
        }

        private async Task createContainerDepartments(SystemResetModel container)
        {
            if (container.Departments == null) return;

            foreach (SystemDepartmentModel model in container.Departments) await _systemDepartmentService.CreateItem(model);
        }

        private async Task<bool> UsernameNotUnique(string username)
        {
            var users = await _systemUsersManager.GetItemsAsync();
            var usernameExists = users.Any(x => x.Username.ToLower() == username.ToLower());
            return usernameExists;
        }
        #endregion Private methods
    }
}
