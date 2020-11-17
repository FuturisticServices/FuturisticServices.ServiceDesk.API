using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    public class SystemServices : SystemBaseService, ISystemService
    {
        private dynamic _response = new ExpandoObject();
        private dynamic _responseContainer = new ExpandoObject();
        private List<dynamic> _responseContainers = new List<dynamic>();
        private List<dynamic> _responseSubscriptions = new List<dynamic>();
        private List<dynamic> _responseUsers = new List<dynamic>();

        private ISystemManager _systemManager;
        private ISystemLookupGroupsManager _systemLookupGroupsManager;
        private ISystemLookupItemsManager _systemLookupItemsManager;
        private ISystemSubscriptionsManager _systemSubscriptionsManager;
        private ISystemUsersManager _systemUsersManager;
        private ISystemTenantsManager _systemTenantsManager;
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemServices(ISystemManager systemManager, ISystemLookupGroupsManager systemLookupGroupManager, ISystemLookupItemsManager systemLookupItemsManager, ISystemSubscriptionsManager systemSubscriptionsManager, ISystemUsersManager systemUsersManager, ISystemTenantsManager systemTenantsManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base()
        {
            _systemManager = systemManager;
            _systemLookupGroupsManager = systemLookupGroupManager;
            _systemLookupItemsManager = systemLookupItemsManager;
            _systemSubscriptionsManager = systemSubscriptionsManager;
            _systemUsersManager = systemUsersManager;
            _systemTenantsManager = systemTenantsManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<object> Reset()
        {
            DatabaseResponse databaseResponse = await _systemManager.CreateDatabase();

            //  Setup _response.
            _response.database = new ExpandoObject();
            _response.database.name = databaseResponse.Resource.Id;
            _response.database.statusCode = databaseResponse.StatusCode;

            if (databaseResponse.StatusCode == HttpStatusCode.Created)
            {
                //  Create containers.
                var containers = _configuration.GetSection("Reset:System:Containers").Get<List<ResetContainerModel>>();

                foreach (ResetContainerModel container in containers)
                {
                    _responseContainer = new ExpandoObject();
                    _responseContainers.Add(_responseContainer);

                    await createContainer(databaseResponse.Database, container);

                    await createLookupGroups(container);

                    await createSubscriptions(container);

                    await createUsers(container);
                }

                _response.database.containers = _responseContainers;

                return _response;
            }

            _response.statusCode = HttpStatusCode.BadRequest;
            _response.value = "System reset not sucessfull.";
            return _response;
        }

        private async Task<ContainerResponse> createContainer(Database database, ResetContainerModel container)
        {
            ContainerResponse containerResponse = await _systemManager.CreateContainer(database, container.Name, container.PartitionKey);

            _responseContainer.name = containerResponse.Resource.Id;
            _responseContainer.statusCode = containerResponse.StatusCode;

            return containerResponse;
        }

        private async Task createLookupGroups(ResetContainerModel container)
        {
            if (container.Groups != null)
            {
                _responseContainer.numberOfItemsCreated = container.Groups.Count();

                List<string> createdGroups = new List<string>();
                foreach (LookupGroup group in container.Groups)
                {
                    await _systemLookupGroupsManager.CreateItemAsync(group);
                    createdGroups.Add(group.Name);
                }
                _responseContainer.items = createdGroups.ToArray();
            }
        }

        private async Task createSubscriptions(ResetContainerModel container)
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

        private async Task createUsers(ResetContainerModel container)
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

        public async Task<IDictionary<string, Object>> ResetTenant()
        {
            var _response = new ExpandoObject() as IDictionary<string, Object>;

            RegistrationModel model = _configuration.GetSection("Reset:Tenant").Get<RegistrationModel>();

            //  Get all lookup items.
            List<LookupGroup> systemLookupItems = (await _systemLookupItemsManager.GetItemsAsync()).ToList();

            //  Get current subscription.
            Subscription currentSubscription = await _systemSubscriptionsManager.GetItemAsync(model.SubscriptionId);

            //  Instantiate and persist tenant to the system tenant container.
            Tenant tenant = new Tenant(model, currentSubscription, systemLookupItems);

            tenant = await _systemTenantsManager.CreateItemAsync(tenant);

            //_response.Add("status (tenant)", new { statusCode = StatusCodes.Status200OK, value = "System tenant created successfully." });
            return _response;
        }
    }
}
