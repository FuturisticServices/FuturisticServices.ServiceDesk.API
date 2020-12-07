using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using FuturisticServices.ServiceDesk.API.Common;
using FuturisticServices.ServiceDesk.API.Entities;
using FuturisticServices.ServiceDesk.API.Managers;
using FuturisticServices.ServiceDesk.API.Models;

namespace FuturisticServices.ServiceDesk.API.Services
{
    /// <summary>
    /// Endpoint interfaces to control/manage the [FuturisticServices.ServiceDesk] database.
    /// </summary>
    public interface ISystemService
    {
        Task<object> Reset();
    }

    /// <summary>
    /// Endpoints to control/manage the [FuturisticServices.ServiceDesk] database.
    /// </summary>
    public class SystemService : SystemBaseService, ISystemService
    {
        private dynamic _response = new ExpandoObject();
        private dynamic _responseContainer = new ExpandoObject();
        private dynamic _responseTenant = new ExpandoObject();
        private List<dynamic> _responseContainers = new List<dynamic>();

        private readonly IHashingService _hashingService;
        private readonly ICosmosDbManager _cosmosDbManager;
        private readonly ISystemManager _systemManager;
        private readonly ISystemLookupGroupManager _systemLookupGroupManager;
        private readonly ISystemLookupItemManager _systemLookupItemsManager;
        private readonly ISystemSubscriptionManager _systemSubscriptionsManager;
        private readonly ISystemUserManager _systemUsersManager;
        private readonly ISystemTenantManager _systemTenantManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Constructor w/ DI for SystemService endpoints.
        /// </summary>
        /// <param name="hashingService">Service for security encryption/decryption.</param>
        /// <param name="cosmosDbManager">Manager to Cosmos DB.</param>
        /// <param name="systemManager">Manager to [FuturisticServices.ServiceDesk] database.</param>
        /// <param name="systemLookupGroupManager">Manager to [FuturustucServices.ServiceDesk].[LookupItems] container from a 'group' perspective.</param>
        /// <param name="systemLookupItemManager">Manager to [FuturustucServices.ServiceDesk].[LookupItems] container from a 'item' perspective.</param>
        /// <param name="systemSubscriptionManager">Manager to [FuturisticServices.ServiceDesk].[Subscriptions] container.</param>
        /// <param name="systemUserManager">Manager to [FuturisticServices.ServiceDesk].[Users] container.</param>
        /// <param name="systemTenantManager">Manager to [FuturisticServices.ServiceDesk].[Tenants] container.</param>
        /// <param name="configuration">Manager to file-based, in-memory and environment variables.</param>
        /// <param name="webHostEnvironment">Manager to the web hosting environment the application is running in.</param>
        public SystemService(IHashingService hashingService, ICosmosDbManager cosmosDbManager, ISystemManager systemManager, ISystemLookupGroupManager systemLookupGroupManager, ISystemLookupItemManager systemLookupItemManager, ISystemSubscriptionManager systemSubscriptionManager, ISystemUserManager systemUserManager, ISystemTenantManager systemTenantManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _hashingService = hashingService;
            _cosmosDbManager = cosmosDbManager;
            _systemManager = systemManager;
            _systemLookupGroupManager = systemLookupGroupManager;
            _systemLookupItemsManager = systemLookupItemManager;
            _systemSubscriptionsManager = systemSubscriptionManager;
            _systemUsersManager = systemUserManager;
            _systemTenantManager = systemTenantManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Resets the [FuturisticServices.ServiceDesk] database by -
        ///     1.  If it exists, deletes the [FuturisticServices.ServiceDesk] database
        ///     2.  Creates the [FuturisticServices.ServiceDesk] database
        ///     3.  Creates the required containers
        ///     4.  Persists the required data to the containers
        /// </summary>
        /// <returns></returns>
        public async Task<object> Reset()
        {
            DatabaseResponse databaseResponse = await _systemManager.CreateDatabase();

            //  Setup _response.
            _response.system = new ExpandoObject();
            _response.system.database = new ExpandoObject();
            _response.system.database.name = databaseResponse.Resource.Id;
            _response.system.database.statusCode = (int)databaseResponse.StatusCode;
            _response.system.database.status = databaseResponse.StatusCode;

            if (databaseResponse.StatusCode == HttpStatusCode.Created)
            {
                //  Create containers.
                var systemContainersToCreate = _configuration.GetSection("system:reset:containers:create").Get<List<ResetContainerModel>>();

                foreach (ResetContainerModel container in systemContainersToCreate)
                {
                    _responseContainer = new ExpandoObject();
                    _responseContainers.Add(_responseContainer);

                    await createContainer(databaseResponse.Database, container);
                    await createContainerLookupGroups(container);
                    await createContainerSubscriptions(container);
                    await createContainerUsers(container);
                }

                _response.system.containers = _responseContainers;

                return _response;
            }

            _response.statusCode = (int)HttpStatusCode.BadRequest;
            _response.status = HttpStatusCode.BadRequest;
            _response.value = "System reset not sucessfull.";
            return _response;
        }

        #region Private methods
        /// <summary>
        /// Creates a container in the [FuturisticServices.ServiceDesk] database.
        /// </summary>
        /// <param name="database">The [FuturisticServices.ServiceDesk] database object.</param>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns>ContainerResponse object</returns>
        private async Task<ContainerResponse> createContainer(Database database, ResetContainerModel container)
        {
            ContainerResponse containerResponse = await _systemManager.CreateContainer(database, container.Name, container.PartitionKey);

            _responseContainer.name = containerResponse.Resource.Id;
            _responseContainer.statusCode = (int)containerResponse.StatusCode;
            _responseContainer.status = containerResponse.StatusCode;

            return containerResponse;
        }

        /// <summary>
        /// Creates multiple items in the [FuturisticServices.ServiceDesk].[LookupItems] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns>ContainerResponse object</returns>
        private async Task createContainerLookupGroups(ResetContainerModel container)
        {
            if (container.Groups != null)
            {
                _responseContainer.numberOfItemsCreated = container.Groups.Count();

                List<string> createdGroups = new List<string>();
                foreach (LookupGroup group in container.Groups)
                {
                    await _systemLookupGroupManager.CreateItemAsync(group);
                    createdGroups.Add(group.Group);
                }
                _responseContainer.items = createdGroups.ToArray();
            }
        }

        /// <summary>
        /// Creates multiple subscriptions in the [FuturisticServices.ServiceDesk].[Subscriptions] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns></returns>
        private async Task createContainerSubscriptions(ResetContainerModel container)
        {
            if (container.Subscriptions != null)
            {
                _responseContainer.numberOfItemsCreated = container.Subscriptions.Count();

                List<string> createdSubscriptions = new List<string>();
                foreach (Subscription subscription in container.Subscriptions)
                {
                    //  If a renewal timeframe exists, retrieve it. Otherwise, make it null.
                    subscription.RenewalTimeframe = subscription.RenewalTimeframe == null ? null : await _systemLookupItemsManager.GetItemAsync("Subscription Renewal Timeframes", Guid.Parse(subscription.RenewalTimeframe.Id));
                    await _systemSubscriptionsManager.CreateItemAsync(subscription);
                    createdSubscriptions.Add(subscription.Name);
                }
                _responseContainer.items = createdSubscriptions;
            }
        }

        /// <summary>
        /// Creates multiple subscriptions in the [FuturisticServices.ServiceDesk].[Users] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns></returns>
        private async Task createContainerUsers(ResetContainerModel container)
        {
            if (container.Users != null)
            {
                _responseContainer.numberOfItemsCreated = container.Users.Count();

                List<string> createdUsers = new List<string>();
                foreach (Entities.User user in container.Users)
                {
                    if (await UsernameUnique(user.Username) == false)
                    {
                        createdUsers.Add(string.Format("{0} {1} not created (username '{2}' not unique)", user.NameFirst, user.NameLast, user.Username));
                    }

                    foreach (EmailAddress userEmailAddress in user.EmailAddresses)
                    {
                        //  Retrieve email address 'type' lookup item.
                        LookupGroup emailAddressesGroup = await _systemLookupItemsManager.GetItemAsync(Enums.LookupGroups.EmailAddressTypes.GetDescription());
                        LookupItem emailAddressType = emailAddressesGroup.Items.SingleOrDefault(x => x.Name == userEmailAddress.Type.Name.ToString().ToTitleCase());
                        userEmailAddress.Type = emailAddressType;
                    }

                    foreach (PhoneNumber userPhoneNumber in user.PhoneNumbers)
                    {
                        //  Retrieve email address 'type' lookup item.
                        LookupGroup phoneNumberGroup = await _systemLookupItemsManager.GetItemAsync(Enums.LookupGroups.PhoneNumberTypes.GetDescription());
                        LookupItem phoneNumberType = phoneNumberGroup.Items.SingleOrDefault(x => x.Name == userPhoneNumber.Type.Name.ToString().ToTitleCase());
                        userPhoneNumber.Type = phoneNumberType;
                    }

                    //  Encrypt password.
                    user.Password = _hashingService.EncryptString(user.Password);

                    //  Persist item.
                    await _systemUsersManager.CreateItemAsync(user);

                    //  Add status to _response.
                    createdUsers.Add(string.Format("{0} {1}", user.NameFirst, user.NameLast));
                }
                _responseContainer.items = createdUsers;
            }
        }

        private async Task<bool> UsernameUnique(string username)
        {
            var users = await _systemUsersManager.GetItemsAsync();
            var usernameExists = users.Any(x => x.Username.ToLower() == username.ToLower());
            return usernameExists;
        }
        #endregion Private methods
    }
}
