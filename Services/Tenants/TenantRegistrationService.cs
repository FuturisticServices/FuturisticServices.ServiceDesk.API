using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Services.System;
using TangledServices.ServicePortal.API.Services.CosmosDb;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ITenantRegistrationService {
        Task Register(TenantRegistrationModel model);
    }

    public class TenantRegistrationService : TenantBaseService, ITenantRegistrationService
    {
        #region Members
        private readonly ITenantSubscriptionService _tenantSubscriptionService;
        private readonly ISystemSubscriptionsService _systemSubscriptionService;
        private readonly ICustomerService _customerService;
        private readonly ISystemLookupItemsService _systemLookupItemService;
        private readonly ITenantLookupItemsService _tenantLookupItemService;
        private readonly ITenantUserService _tenantUserService;
        private readonly IHashingService _hashingService;
        private readonly ICosmosDbService _cosmosDbService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public TenantRegistrationService(ITenantSubscriptionService tenantSubscriptionService, ISystemSubscriptionsService systemSubscriptionService, ICustomerService customerService, ISystemLookupItemsService systemLookupItemService, ITenantLookupItemsService tenantLookupItemService, ITenantUserService tenantUserService, IHashingService hashingService, ICosmosDbService cosmosDbService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _tenantSubscriptionService = tenantSubscriptionService;
            _systemSubscriptionService = systemSubscriptionService;
            _customerService = customerService;
            _systemLookupItemService = systemLookupItemService;
            _tenantLookupItemService = tenantLookupItemService;
            _tenantUserService = tenantUserService;
            _hashingService = hashingService;
            _cosmosDbService = cosmosDbService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region Public methods
        public async Task Register(TenantRegistrationModel model)
        {
            //Subscription systemSubscription = await _systemSubscriptionService.GetItem(model.SystemTenant.Subscription.Id);

            //await _systemTenantsService.Create(model.SystemTenant);

            //if (await _systemSubscriptionService.NotFound(model.SystemTenant.Subscription.Id)) throw new SubscriptionNotFoundException(model.SystemTenant.Subscription.Id);

            //CustomerEntity systemTenant = await _systemTenantsService.Get(model.Moniker);

            //DatabaseResponse databaseResponse = await _cosmosDbService.CreateDatabase(systemTenant);
            //if (databaseResponse.StatusCode != HttpStatusCode.Created && databaseResponse.StatusCode != HttpStatusCode.OK) throw new TenantRegistrationDatabaseCouldNotBeCreatedException(model.Moniker);

            ////  Retrieve tenant containers to create.
            //var containers = _configuration.GetSection("tenant:setup:containers:create").Get<List<DatabaseContainer>>(); //  from tenant-setup.json
            //if (!containers.Any()) throw new TenantRegistrationNoContainersFoundToCreateException();

            ////  Retrieve lookup groups to omit.
            //var omitLookupGroups = _configuration.GetSection("tenant:setup:lookupGroups:omit").GetChildren().Select(x => x.Value).ToList(); //  from tenant-setup.json

            //foreach (DatabaseContainer container in containers)
            //{
            //    ContainerResponse containerResponse = await _cosmosDbService.CreateContainer(databaseResponse.Database, container.Name, container.PartitionKey);
            //    if (containerResponse.StatusCode != HttpStatusCode.Created && containerResponse.StatusCode != HttpStatusCode.OK) throw new TenantRegistrationCreateContainerFailedException(container.Name);

            //    if (container.CloneItems)
            //    {
            //        //  Populate lookup items from system database.
            //        List<LookupGroupEntity> lookupGroups = (await _systemLookupItemService.GetItems()).ToList();
            //        foreach (LookupGroupEntity group in lookupGroups)
            //        {
            //            //  Skip over groups to ommit.
            //            if (omitLookupGroups.Contains(group.Group)) continue;

            //            //  Overwrite existing ID with new GUID unique to tenant.
            //            group.Items.ToList().ForEach(x => x.Id = Guid.NewGuid().ToString());

            //            //  Persist item.
            //            await _tenantLookupItemService.CreateItem(group);
            //        }
            //    }
            //}

            ////  Create tenant subscription.
            //TenantSubscriptionModel tenantSubscriptionModel = new TenantSubscriptionModel(systemSubscription, DateTime.Now) { IsActive = true };
            //TenantSubscription tenantSubscription = new TenantSubscription(tenantSubscriptionModel);
            //tenantSubscription = await _tenantSubscriptionService.CreateItem(tenantSubscription);

            ////  Create tenant users. 
            //var users = _configuration.GetSection("tenant:setup:users").Get<List<UserModel>>(); //  from tenant-setup.json

            ////  Persist tenant users.
            //foreach (UserModel user in users)
            //{
            //    user.Id = Guid.NewGuid().ToString();
            //    user.EmployeeId = user.Username == "tangled.admin" ? user.EmployeeId.Replace("{moniker}", model.Moniker) : await _tenantUserService.GetUniqueEmployeeId(model.Moniker);
            //    user.Username = user.Username.Replace("{moniker}", model.Moniker);
            //    user.Password = _hashingService.EncryptString(user.Password);

            //    //  Complete each email address.
            //    foreach (EmailAddressModel emailAddress in user.EmailAddresses)
            //    {
            //        emailAddress.Address = emailAddress.Address.Replace("{moniker}", model.Moniker);
            //        emailAddress.Type = await _tenantLookupItemService.GetItem("Email Address Types", emailAddress.Type.Id);
            //    }

            //    //  Complete each phone number.
            //    foreach (PhoneNumberModel phoneNumber in user.PhoneNumbers)
            //    {
            //        phoneNumber.Type = await _tenantLookupItemService.GetItem("Phone Number Types", phoneNumber.Type.Id);
            //    }

            //    Entities.User userEntity = new Entities.User(user);

            //    //  Persist item.
            //    await _tenantUserService.CreateItem(userEntity);
            //}
        }
        #endregion Public methods
    }
}