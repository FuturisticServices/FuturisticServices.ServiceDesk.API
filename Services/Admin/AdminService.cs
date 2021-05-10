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
using TangledServices.ServicePortal.API.Services;

namespace TangledServices.ServicePortal.API.Services
{
    /// <summary>
    /// Endpoint interfaces to control/manage the [TangledServices.ServicePortal] database.
    /// </summary>
    public interface IAdminService
    {
        Task<DatabaseResponse> CreateDatabase();
        Task<ContainerResponse> CreateContainer(string name, string partitionKey);
    }

    /// <summary>
    /// Endpoints to control/manage the [TangledServices.ServicePortal] database.
    /// </summary>
    public class AdminService : AdminBaseService, IAdminService
    {
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly ICosmosDbManager _cosmosDbManager;
        private readonly IAdminManager _adminManager;
        private readonly IAdminLookupItemsManager _adminLookupItemsManager;
        private readonly IAdminUsersManager _customerAdminUsersManager;
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminService(ISystemLookupItemService systemLookupItemService, ICosmosDbManager cosmosDbManager, IAdminManager adminManager, IAdminLookupItemsManager adminLookupItemsManager, IAdminUsersManager customerUserAdminsManager, IHashingService hashingService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemLookupItemService = systemLookupItemService;
            _cosmosDbManager = cosmosDbManager;
            _adminManager = adminManager;
            _adminLookupItemsManager = adminLookupItemsManager;
            _customerAdminUsersManager = customerUserAdminsManager;
            _hashingService = hashingService;
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
        public async Task<DatabaseResponse> CreateDatabase()
        {
            var databaseResponse = await _adminManager.CreateDatabase();
            if (databaseResponse.StatusCode != HttpStatusCode.Created) throw new AdminDatabaseNotCreatedException();
            return databaseResponse;
        }

        public async Task<ContainerResponse> CreateContainer(string name, string partitionKey)
        {
            return await _adminManager.CreateContainer(name, partitionKey);
        }

        #region Private methods
        /// <summary>
        /// Creates a container in the [TangledServices.ServicePortal] database.
        /// </summary>
        /// <param name="database">The [TangledServices.ServicePortal] database object.</param>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns>ContainerResponse object</returns>
        private async Task<ContainerResponse> CreateContainer( SystemLookupItem group)
        {
            ContainerResponse containerResponse = await _adminManager.CreateContainer(group.Name, group.Name);
            return containerResponse;
        }

        /// <summary>
        /// Creates multiple items in the [TangledServices.ServicePortal].[LookupItems] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns>ContainerResponse object</returns>
        private async Task CreateContainerLookupGroups(SystemLookupItem container)
        {
            //if (container.Groups != null)
            //{
            //    foreach (LookupGroup group in container.Groups)
            //    {
            //        await _adminLookupItemManager.CreateItemAsync(group);
            //    }
            //}
        }

        ///// <summary>
        ///// Creates multiple subscriptions in the [TangledServices.ServicePortal].[Subscriptions] container.
        ///// </summary>
        ///// <param name="container">The container represented as a ResetContainerModel object.</param>
        ///// <returns></returns>
        //private async Task<IEnumerable<Subscription>> createContainerSubscriptions(SystemResetModel container)
        //{
        //    List<Subscription> items = new List<Subscription>();

        //    if (container.Subscriptions != null)
        //    {
        //        foreach (Subscription subscription in container.Subscriptions)
        //        {
        //            //  If a renewal timeframe exists, retrieve it. Otherwise, make it null.
        //            subscription.RenewalTimeframe = subscription.RenewalTimeframe == null ? null : await _adminLookupItemManager.GetItemAsync("Subscription Renewal Timeframes", subscription.RenewalTimeframe.Id);
        //            var item = await _customerAdminSubscriptionsManager.CreateItemAsync(subscription);
        //            items.Add(item);
        //        }
        //    }

        //    return items;
        //}

        /// <summary>
        /// Creates multiple subscriptions in the [TangledServices.ServicePortal].[Users] container.
        /// </summary>
        /// <param name="container">The container represented as a ResetContainerModel object.</param>
        /// <returns></returns>
        private async Task CreateContainerUsers(SystemLookupItem container)
        {
            //if (container.Users != null)
            //{
            //    foreach (Entities.User user in container.Users)
            //    {
            //        foreach (EmailAddress userEmailAddress in user.EmailAddresses)
            //        {
            //            //  Retrieve email address lookup items.
            //            LookupGroup emailAddressesGroup = await _systemLookupItemService.GetItem(Enums.LookupGroups.EmailAddressTypes);
            //            LookupItemEntity emailAddressType = emailAddressesGroup.Items.SingleOrDefault(x => x.Name == userEmailAddress.Type.Name.ToString().ToTitleCase());
            //            userEmailAddress.Type = emailAddressType;
            //        }

            //        foreach (PhoneNumber userPhoneNumber in user.PhoneNumbers)
            //        {
            //            //  Retrieve phone number lookup items.
            //            LookupGroup phoneNumberGroup = await _systemLookupItemService.GetItem(Enums.LookupGroups.PhoneNumberTypes);
            //            LookupItemEntity phoneNumberType = phoneNumberGroup.Items.SingleOrDefault(x => x.Name == userPhoneNumber.Type.Name.ToString().ToTitleCase());
            //            userPhoneNumber.Type = phoneNumberType;
            //        }

            //        //  Encrypt password.
            //        user.Password = _hashingService.EncryptString(user.Password);

            //        //  Persist item.
            //        var item = await _customerAdminUserManager.CreateItemAsync(user);
            //    }
            //}
        }
        #endregion Private methods
    }
}
