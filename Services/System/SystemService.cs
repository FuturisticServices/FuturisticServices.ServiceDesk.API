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
        private readonly ICosmosDbManager _cosmosDbManager;
        private readonly ISystemManager _systemManager;
        private readonly ISystemLookupItemsService _systemLookupItemsService;
        private readonly ISystemLookupItemsManager _systemLookupItemsManager;
        private readonly ISystemSubscriptionsManager _systemSubscriptionsManager;
        private readonly ISystemUsersManager _systemUsersManager;
        private readonly ICustomersManager _systemTenantsManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Constructor w/ DI for SystemService endpoints.
        /// </summary>
        /// <param name="hashingService">Service for security encryption/decryption.</param>
        /// <param name="cosmosDbManager">Manager to Cosmos DB.</param>
        /// <param name="systemManager">Manager to [TangledServices.ServicePortal] database.</param>
        /// <param name="systemLookupItemsManager">Manager to [FuturustucServices.ServicePortal].[LookupItems] container from a 'item' perspective.</param>
        /// <param name="systemSubscriptionManager">Manager to [TangledServices.ServicePortal].[Subscriptions] container.</param>
        /// <param name="systemUsersManager">Manager to [TangledServices.ServicePortal].[Users] container.</param>
        /// <param name="systemTenantsManager">Manager to [TangledServices.ServicePortal].[Tenants] container.</param>
        /// <param name="configuration">Manager to file-based, in-memory and environment variables.</param>
        /// <param name="webHostEnvironment">Manager to the web hosting environment the application is running in.</param>
        public SystemService(ISystemUsersService systemUsersService, IHashingService hashingService, ICosmosDbManager cosmosDbManager, ISystemManager systemManager, ISystemLookupItemsService systemLookupItemsService, ISystemLookupItemsManager systemLookupItemsManager, ISystemSubscriptionsManager systemSubscriptionManager, ISystemUsersManager systemUsersManager, ICustomersManager systemTenantsManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemUsersService = systemUsersService;
            _hashingService = hashingService;
            _cosmosDbManager = cosmosDbManager;
            _systemManager = systemManager;
            _systemLookupItemsService = systemLookupItemsService;
            _systemLookupItemsManager = systemLookupItemsManager;
            _systemSubscriptionsManager = systemSubscriptionManager;
            _systemUsersManager = systemUsersManager;
            _systemTenantsManager = systemTenantsManager;
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
            ContainerResponse containerResponse = await _systemManager.CreateContainer(container.Name, container.PartitionKey);
            return containerResponse;
        }

        /// <summary>
        /// Creates multiple items in the [TangledServices.ServicePortal].[LookupItems] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns>ContainerResponse object</returns>
        private async Task createContainerLookupItems(SystemResetModel container)
        {
            if (container.Items != null)
            {
                foreach (LookupItemModel model in container.Items)
                {
                    LookupItem entity = new LookupItem(model);
                    await _systemLookupItemsManager.CreateItemAsync(entity);
                }
            }
        }

        /// <summary>
        /// Creates multiple subscriptions in the [TangledServices.ServicePortal].[Subscriptions] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns></returns>
        private async Task createContainerSubscriptions(SystemResetModel container)
        {
            if (container.Subscriptions != null)
            {
                foreach (SubscriptionModel model in container.Subscriptions)
                {
                    //  If a renewal timeframe exists, retrieve it. Otherwise, make it null.
                    model.RenewalTimeframe = model.RenewalTimeframe == null ? null : await _systemLookupItemsManager.GetItemAsync("Subscription Renewal Timeframes", model.RenewalTimeframe.Id);
                    var entity = new Subscription(model);
                    await _systemSubscriptionsManager.CreateItemAsync(entity);
                }
            }
        }

        /// <summary>
        /// Creates multiple subscriptions in the [TangledServices.ServicePortal].[Users] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns></returns>
        private async Task createContainerUsers(SystemResetModel container)
        {
            if (container.Users != null)
            {
                foreach (SystemUserModel model in container.Users)
                {
                    foreach (EmailAddressModel emailAddressModel in model.EmailAddresses)
                    {
                        emailAddressModel.Id = Guid.NewGuid().ToString();
                        var emailAddress = await _systemLookupItemsService.GetItem(Enums.LookupItems.EmailAddressTypes.GetDescription().ToTitleCase(), emailAddressModel.Type.Id);
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

                    SystemUser entity = new SystemUser(model);

                    //  Persist item.
                    var item = await _systemUsersManager.CreateItemAsync(entity);
                }
            }
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
