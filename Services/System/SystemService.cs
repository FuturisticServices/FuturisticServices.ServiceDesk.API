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
    public interface ISystemService
    {
        Task<object> Reset();
    }

    public class SystemService : SystemBaseService, ISystemService
    {
        private Tenant _tenant = null;

        private dynamic _response = new ExpandoObject();
        private dynamic _responseContainer = new ExpandoObject();
        private dynamic _responseTenant = new ExpandoObject();
        private List<dynamic> _responseContainers = new List<dynamic>();

        private readonly ICosmosDbManager _cosmosDbManager;
        private readonly ISystemManager _systemManager;
        private readonly ISystemLookupGroupManager _systemLookupGroupManager;
        private readonly ISystemLookupItemManager _systemLookupItemsManager;
        private readonly ISystemSubscriptionManager _systemSubscriptionsManager;
        private readonly ISystemUserManager _systemUsersManager;
        private readonly ISystemTenantManager _systemTenantManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemService(ICosmosDbManager cosmosDbManager, ISystemManager systemManager, ISystemLookupGroupManager systemLookupGroupManager, ISystemLookupItemManager systemLookupItemManager, ISystemSubscriptionManager systemSubscriptionManager, ISystemUserManager systemUserManager, ISystemTenantManager systemTenantManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
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
                var systemContainersToCreate = _configuration.GetSection("system:setup:containers:create").Get<List<ResetContainerModel>>();

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
        private async Task<ContainerResponse> createContainer(Database database, ResetContainerModel container)
        {
            ContainerResponse containerResponse = await _systemManager.CreateContainer(database, container.Name, container.PartitionKey);

            _responseContainer.name = containerResponse.Resource.Id;
            _responseContainer.statusCode = (int)containerResponse.StatusCode;
            _responseContainer.status = containerResponse.StatusCode;

            return containerResponse;
        }

        private async Task createContainerLookupGroups(ResetContainerModel container)
        {
            if (container.Groups != null)
            {
                _responseContainer.numberOfItemsCreated = container.Groups.Count();

                List<string> createdGroups = new List<string>();
                foreach (LookupGroup group in container.Groups)
                {
                    await _systemLookupGroupManager.CreateItemAsync(group);
                    createdGroups.Add(group.Name);
                }
                _responseContainer.items = createdGroups.ToArray();
            }
        }

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

        private async Task createContainerUsers(ResetContainerModel container)
        {
            if (container.Users != null)
            {
                _responseContainer.numberOfItemsCreated = container.Users.Count();

                List<string> createdUsers = new List<string>();
                foreach (Entities.User user in container.Users)
                {
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

                    //  Persist item.
                    await _systemUsersManager.CreateItemAsync(user);

                    //  Add status to _response.
                    createdUsers.Add(string.Format("{0} {1}", user.NameFirst, user.NameLast));
                }
                _responseContainer.items = createdUsers;
            }
        }

        //private async Task createContainerTenants(ResetContainerModel container)
        //{
        //    if (container.Tenants != null)
        //    {
        //        _responseContainer.numberOfItemsCreated = container.Tenants.Count();

        //        List<string> createdTenants = new List<string>();
        //        foreach (Tenant model in container.Tenants)
        //        {
        //           // Get all lookup items.
        //           List<LookupGroup> systemLookupItems = (await _systemLookupItemsManager.GetItemsAsync()).ToList();

        //            //  Get current subscription.
        //            Subscription subscription = await _systemSubscriptionsManager.GetItemAsync(model.SubscriptionId);

        //            var tenant = new Tenant(model, subscription, systemLookupItems);

        //            await _systemTenantsManager.CreateItemAsync(tenant);
        //            createdTenants.Add(tenant.Company.Name);
        //        }
        //        _responseContainer.items = createdTenants;
        //    }
        //}

        //private async Task createTenants(ResetContainerModel container)
        //{
        //    if (container.Tenants != null)
        //    {
        //        //_responseContainer.numberOfItemsCreated = container.Tenants.Count();

        //        List<string> createdTenants = new List<string>();
        //        foreach (RegistrationModel model in container.Tenants)
        //        {
        //            _responseTenant = new ExpandoObject();

        //            //  Get all lookup items.
        //            List<LookupGroup> systemLookupItems = (await _systemLookupItemsManager.GetItemsAsync()).ToList();

        //            //  Get current subscription.
        //            Subscription subscription = await _systemSubscriptionsManager.GetItemAsync(model.SubscriptionId);

        //            _tenant = new Tenant(model, subscription, systemLookupItems);

        //            DatabaseResponse databaseResponse = await _cosmosDbManager.CreateDatabaseAsync(_tenant);

        //            if (databaseResponse.StatusCode == HttpStatusCode.Created)
        //            {
        //                var databaseTenant = databaseResponse.Database;
        //                CloneSystemContainers(databaseTenant, _responseTenant);
        //            }

        //            _responseTenant.name = _tenant.Company.Name;
        //            _responseTenant.moniker = _tenant.Moniker;
        //            _responseTenant.database = new ExpandoObject();
        //            _responseTenant.database.name = databaseResponse.Resource.Id;
        //            _responseTenant.database.statusCode = (int)databaseResponse.StatusCode;
        //            _responseTenant.database.status = databaseResponse.StatusCode;

        //            //  Add status to _response.
        //            createdTenants.Add(_tenant.Company.Name);
        //        }
        //    }
        //}

        //private async Task CloneSystemContainers(Database databaseTenant, ExpandoObject _responseTenant)
        //{
        //    var containersToClone = _configuration.GetSection("reset:system:containersToClone").Get<List<string>>();
        //    var lookupGroupsToClone = _configuration.GetSection("reset:system:lookupGroupsToClone").Get<List<string>>();

        //    var groups = await _systemManager.GetContainers(lookupGroupsToClone);

        //    foreach (string group in groups) {
        //        await _cosmosDbManager.CreateContainerAsync(databaseTenant, group, "name");

        //        if (lookupGroupsToClone.Contains(group))
        //        {
        //            await CreateTenanLookupGroups(group);
        //        }
        //    } 
        //}

        //private async Task CreateTenanLookupGroups(string groupName)
        //{
        //    LookupGroup group = await _systemLookupItemsManager.GetItemAsync(groupName);

        //    foreach (LookupItem item in group.Items)
        //    {
                
        //    }
        //}
        #endregion Private methods
    }
}
