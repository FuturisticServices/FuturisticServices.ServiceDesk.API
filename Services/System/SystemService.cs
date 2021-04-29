using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

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
        private readonly IHashingService _hashingService;
        private readonly ISystemUsersService _systemUsersService;
        private readonly ISystemLookupItemsService _systemLookupItemsService;
        private readonly ISystemSubscriptionsService _systemSubscriptionsService;
        private readonly ISystemDepartmentsService _systemDepartmentsService;

        private readonly ISystemManager _systemManager;
        private readonly ISystemUsersManager _systemUsersManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="systemUsersService"></param>
        /// <param name="hashingService"></param>
        /// <param name="systemManager"></param>
        /// <param name="systemLookupItemsService"></param>
        /// <param name="systemSubscriptionsService"></param>
        /// <param name="systemUsersManager"></param>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        public SystemService(ISystemUsersService systemUsersService, 
            IHashingService hashingService, 
            ISystemManager systemManager, 
            ISystemLookupItemsService systemLookupItemsService, 
            ISystemSubscriptionsService systemSubscriptionsService, 
            ISystemDepartmentsService systemDepartmentsService,
            ISystemUsersManager systemUsersManager, 
            IConfiguration configuration, 
            IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemUsersService = systemUsersService;
            _hashingService = hashingService;
            _systemManager = systemManager;
            _systemLookupItemsService = systemLookupItemsService;
            _systemSubscriptionsService = systemSubscriptionsService;
            _systemDepartmentsService = systemDepartmentsService;
            _systemUsersManager = systemUsersManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Resets the [TangledServices.ServicePortal] database by -
        ///     1.  If it exists, deletes the [TangledServices.ServicePortal] database
        ///     2.  Creates the [TangledServices.ServicePortal] database
        ///     3.  Creates the required containers
        ///     4.  Persists the required data to the containers
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

        public async Task<IEnumerable<Container>> GetContainers()
        {
            var response = await _systemManager.GetContainers();
            return response;
        }

        public Container GetContainer(string name)
        {
            Container response = _systemManager.GetContainer(name);
            return response;
        }

        public bool ContainerExists(string name)
        {
            Container container = _systemManager.GetContainer(name);
            return container != null;
        }

        #region Private methods
        /// <summary>
        /// Creates a container in the [TangledServices.ServicePortal] database.
        /// </summary>
        /// <param name="database">The [TangledServices.ServicePortal] database object.</param>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns>ContainerResponse object</returns>
        private async Task<ContainerResponse> createContainer(SystemResetModel container)
        {
            ContainerResponse containerResponse = await _systemManager.CreateContainer(container.Name, container.PartitionKeyPath);
            return containerResponse;
        }

        /// <summary>
        /// Creates multiple items in the [TangledServices.ServicePortal].[LookupItems] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns>ContainerResponse object</returns>
        private async Task createContainerLookupItems(SystemResetModel container)
        {
            if (container.LookupItems == null) return;

            foreach (LookupItemModel model in container.LookupItems)
            {
                await _systemLookupItemsService.CreateItem(model);
            }
        }

        /// <summary>
        /// Creates multiple subscriptions in the [TangledServices.ServicePortal].[Subscriptions] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns></returns>
        private async Task createContainerSubscriptions(SystemResetModel container)
        {
            if (container.Subscriptions == null) return;

            foreach (SubscriptionModel model in container.Subscriptions)
            {
                //  If a renewal timeframe exists, retrieve it. Otherwise, make it null.
                model.RenewalTimeframe = model.RenewalTimeframe == null ? null : await _systemLookupItemsService.GetItem("Subscription Renewal Timeframes", model.RenewalTimeframe.Id);
                var entity = new Subscription(model);
                await _systemSubscriptionsService.CreateItem(entity);
            }
        }

        /// <summary>
        /// Creates multiple subscriptions in the [TangledServices.ServicePortal].[Users] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns></returns>
        private async Task createContainerUsers(SystemResetModel container)
        {
            if (container.Users == null) return;

            foreach (SystemUserModel model in container.Users)
            {
                foreach (EmailAddressModel emailAddressModel in model.EmailAddresses)
                {
                    emailAddressModel.Id = Guid.NewGuid().ToString();
                    var emailAddress = await _systemLookupItemsService.GetItem(Enums.LookupItems.EmailAddressTypes.GetDescription().ToTitleCase(), emailAddressModel.Type.Id);
                    //  TODO: Throw exception if emailAddress == null
                    emailAddressModel.Type = new LookupItemValueModel(emailAddress);
                }

                foreach (PhoneNumberModel phoneNumberModel in model.PhoneNumbers)
                {
                    phoneNumberModel.Id = Guid.NewGuid().ToString();
                    var phoneNumber = await _systemLookupItemsService.GetItem(Enums.LookupItems.PhoneNumberTypes.GetDescription().ToTitleCase(), phoneNumberModel.Type.Id);
                    phoneNumberModel.Type = new LookupItemValueModel(phoneNumber);
                }

                //  Encrypt password.
                model.Password = _hashingService.EncryptString(model.Password);

                //  Create entity from model.
                SystemUser entity = new SystemUser(model);

                //  Persist item.
                await _systemUsersService.CreateItem(entity);
            }
        }

        private async Task createContainerDepartments(SystemResetModel container)
        {
            if (container.Departments == null) return;

            foreach (SystemDepartmentModel model in container.Departments) await _systemDepartmentsService.CreateItem(model);
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
