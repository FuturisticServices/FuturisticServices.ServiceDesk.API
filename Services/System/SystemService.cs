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
        private readonly ISystemLookupItemManager _systemLookupItemManager;
        private readonly ISystemSubscriptionManager _systemSubscriptionsManager;
        private readonly ISystemUsersManager _systemUsersManager;
        private readonly ISystemTenantsManager _systemTenantsManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Constructor w/ DI for SystemService endpoints.
        /// </summary>
        /// <param name="hashingService">Service for security encryption/decryption.</param>
        /// <param name="cosmosDbManager">Manager to Cosmos DB.</param>
        /// <param name="systemManager">Manager to [TangledServices.ServicePortal] database.</param>
        /// <param name="systemLookupItemManager">Manager to [FuturustucServices.ServicePortal].[LookupItems] container from a 'item' perspective.</param>
        /// <param name="systemSubscriptionManager">Manager to [TangledServices.ServicePortal].[Subscriptions] container.</param>
        /// <param name="systemUsersManager">Manager to [TangledServices.ServicePortal].[Users] container.</param>
        /// <param name="systemTenantsManager">Manager to [TangledServices.ServicePortal].[Tenants] container.</param>
        /// <param name="configuration">Manager to file-based, in-memory and environment variables.</param>
        /// <param name="webHostEnvironment">Manager to the web hosting environment the application is running in.</param>
        public SystemService(ISystemUsersService systemUsersService, IHashingService hashingService, ICosmosDbManager cosmosDbManager, ISystemManager systemManager, ISystemLookupItemManager systemLookupItemManager, ISystemSubscriptionManager systemSubscriptionManager, ISystemUsersManager systemUsersManager, ISystemTenantsManager systemTenantsManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemUsersService = systemUsersService;
            _hashingService = hashingService;
            _cosmosDbManager = cosmosDbManager;
            _systemManager = systemManager;
            _systemLookupItemManager = systemLookupItemManager;
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
            var systemContainersToCreate = _configuration.GetSection("system:reset:containers").Get<List<ResetContainerModel>>(); //  from system-reset.json

            //  Persist data to containers.
            foreach (ResetContainerModel container in systemContainersToCreate)
            {
                var containerResponse = await createContainer(databaseResponse.Database, container);
                var lookupGroupEntities = await createContainerLookupGroups(container);
                var subscriptions = await createContainerSubscriptions(container);
                var users = await createContainerUsers(container);
            }
        }

        #region Private methods
        /// <summary>
        /// Creates a container in the [TangledServices.ServicePortal] database.
        /// </summary>
        /// <param name="database">The [TangledServices.ServicePortal] database object.</param>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns>ContainerResponse object</returns>
        private async Task<ContainerResponse> createContainer(Database database, ResetContainerModel container)
        {
            ContainerResponse containerResponse = await _systemManager.CreateContainer(database, container.Name, container.PartitionKey);
            return containerResponse;
        }

        /// <summary>
        /// Creates multiple items in the [TangledServices.ServicePortal].[LookupItems] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns>ContainerResponse object</returns>
        private async Task<IEnumerable<LookupGroupEntity>> createContainerLookupGroups(ResetContainerModel container)
        {
            List<LookupGroupEntity> items = new List<LookupGroupEntity>();

            if (container.Groups != null)
            {
                foreach (LookupGroupEntity group in container.Groups)
                {
                    var item = await _systemLookupItemManager.CreateItemAsync(group);
                    items.Add(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Creates multiple subscriptions in the [TangledServices.ServicePortal].[Subscriptions] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns></returns>
        private async Task<IEnumerable<Subscription>> createContainerSubscriptions(ResetContainerModel container)
        {
            List<Subscription> items = new List<Subscription>();

            if (container.Subscriptions != null)
            {
                foreach (Subscription subscription in container.Subscriptions)
                {
                    //  If a renewal timeframe exists, retrieve it. Otherwise, make it null.
                    subscription.RenewalTimeframe = subscription.RenewalTimeframe == null ? null : await _systemLookupItemManager.GetItemAsync("Subscription Renewal Timeframes", subscription.RenewalTimeframe.Id);
                    var item = await _systemSubscriptionsManager.CreateItemAsync(subscription);
                    items.Add(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Creates multiple subscriptions in the [TangledServices.ServicePortal].[Users] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns></returns>
        private async Task<IEnumerable<Entities.User>> createContainerUsers(ResetContainerModel container)
        {
            List<Entities.User> items = new List<Entities.User>();

            if (container.Users != null)
            {
                foreach (Entities.User user in container.Users)
                {
                    foreach (EmailAddress userEmailAddress in user.EmailAddresses)
                    {
                        //  Retrieve email address 'type' lookup item.
                        LookupGroupEntity emailAddressesGroup = await _systemLookupItemManager.GetItemAsync(Enums.LookupGroups.EmailAddressTypes.GetDescription());
                        LookupItemEntity emailAddressType = emailAddressesGroup.Items.SingleOrDefault(x => x.Name == userEmailAddress.Type.Name.ToString().ToTitleCase());
                        userEmailAddress.Type = emailAddressType;
                    }

                    foreach (PhoneNumber userPhoneNumber in user.PhoneNumbers)
                    {
                        //  Retrieve email address 'type' lookup item.
                        LookupGroupEntity phoneNumberGroup = await _systemLookupItemManager.GetItemAsync(Enums.LookupGroups.PhoneNumberTypes.GetDescription());
                        LookupItemEntity phoneNumberType = phoneNumberGroup.Items.SingleOrDefault(x => x.Name == userPhoneNumber.Type.Name.ToString().ToTitleCase());
                        userPhoneNumber.Type = phoneNumberType;
                    }

                    //  Encrypt password.
                    user.Password = _hashingService.EncryptString(user.Password);

                    //  Persist item.
                    var item = await _systemUsersManager.CreateItemAsync(user);

                    //  Add status to _response.
                    items.Add(item);
                }
            }

            return items;
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
