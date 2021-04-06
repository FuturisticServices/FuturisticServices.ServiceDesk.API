using System;
using System.Collections.Generic;
using System.Dynamic;
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
        Task<object> Reset();
    }

    /// <summary>
    /// Endpoints to control/manage the [TangledServices.ServicePortal] database.
    /// </summary>
    public class SystemService : SystemBaseService, ISystemService
    {
        private readonly IHashingService _hashingService;
        private readonly ICosmosDbManager _cosmosDbManager;
        private readonly ISystemManager _systemManager;
        private readonly ISystemLookupItemManager _systemLookupItemManager;
        //private readonly ISystemLookupItemManager _systemLookupItemsManager;
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
        /// <param name="systemManager">Manager to [TangledServices.ServicePortal] database.</param>
        /// <param name="systemLookupItemManager">Manager to [FuturustucServices.ServicePortal].[LookupItems] container from a 'item' perspective.</param>
        /// <param name="systemSubscriptionManager">Manager to [TangledServices.ServicePortal].[Subscriptions] container.</param>
        /// <param name="systemUserManager">Manager to [TangledServices.ServicePortal].[Users] container.</param>
        /// <param name="systemTenantManager">Manager to [TangledServices.ServicePortal].[Tenants] container.</param>
        /// <param name="configuration">Manager to file-based, in-memory and environment variables.</param>
        /// <param name="webHostEnvironment">Manager to the web hosting environment the application is running in.</param>
        public SystemService(IHashingService hashingService, ICosmosDbManager cosmosDbManager, ISystemManager systemManager, ISystemLookupItemManager systemLookupItemManager, ISystemSubscriptionManager systemSubscriptionManager, ISystemUserManager systemUserManager, ISystemTenantManager systemTenantManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _hashingService = hashingService;
            _cosmosDbManager = cosmosDbManager;
            _systemManager = systemManager;
            _systemLookupItemManager = systemLookupItemManager;
            _systemSubscriptionsManager = systemSubscriptionManager;
            _systemUsersManager = systemUserManager;
            _systemTenantManager = systemTenantManager;
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
        public async Task<object> Reset()
        {
            DatabaseResponse databaseResponse = await _systemManager.CreateDatabase();

            if (databaseResponse.StatusCode == HttpStatusCode.Created)
            {
                //  Create containers.
                var systemContainersToCreate = _configuration.GetSection("system:reset:containers").Get<List<ResetContainerModel>>();

                foreach (ResetContainerModel container in systemContainersToCreate)
                {
                    await createContainer(databaseResponse.Database, container);
                    await createContainerLookupGroups(container);
                    await createContainerSubscriptions(container);
                    await createContainerUsers(container);
                }

                _responseSuccess.StatusCode = (int)HttpStatusCode.OK;
                _responseSuccess.Description = "System reset sucessfull.";
                return _responseSuccess;
            }

            _responseSuccess.StatusCode = (int)HttpStatusCode.BadRequest;
            _responseSuccess.Description = "System reset NOT sucessfull.";
            return _responseSuccess;
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

            //_responseContainer.name = containerResponse.Resource.Id;
            //_responseContainer.statusCode = (int)containerResponse.StatusCode;
            //_responseContainer.status = containerResponse.StatusCode;

            return containerResponse;
        }

        /// <summary>
        /// Creates multiple items in the [TangledServices.ServicePortal].[LookupItems] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns>ContainerResponse object</returns>
        private async Task createContainerLookupGroups(ResetContainerModel container)
        {
            if (container.Groups != null)
            {
                List<string> createdGroups = new List<string>();
                foreach (LookupGroupEntity group in container.Groups)
                {
                    await _systemLookupItemManager.CreateItemAsync(group);
                    createdGroups.Add(group.Group);
                }
            }
        }

        /// <summary>
        /// Creates multiple subscriptions in the [TangledServices.ServicePortal].[Subscriptions] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns></returns>
        private async Task createContainerSubscriptions(ResetContainerModel container)
        {
            if (container.Subscriptions != null)
            {
                //_responseContainer.numberOfItemsCreated = container.Subscriptions.Count();

                List<string> createdSubscriptions = new List<string>();
                foreach (Subscription subscription in container.Subscriptions)
                {
                    //  If a renewal timeframe exists, retrieve it. Otherwise, make it null.
                    subscription.RenewalTimeframe = subscription.RenewalTimeframe == null ? null : await _systemLookupItemManager.GetItemAsync("Subscription Renewal Timeframes", subscription.RenewalTimeframe.Id);
                    await _systemSubscriptionsManager.CreateItemAsync(subscription);
                    createdSubscriptions.Add(subscription.Name);
                }
                //_responseContainer.items = createdSubscriptions;
            }
        }

        /// <summary>
        /// Creates multiple subscriptions in the [TangledServices.ServicePortal].[Users] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns></returns>
        private async Task createContainerUsers(ResetContainerModel container)
        {
            if (container.Users != null)
            {
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
                    await _systemUsersManager.CreateItemAsync(user);

                    //  Add status to _response.
                    createdUsers.Add(string.Format("{0} {1}", user.NameFirst, user.NameLast));
                }
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
