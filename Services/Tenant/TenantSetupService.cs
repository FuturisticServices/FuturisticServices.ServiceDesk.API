using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
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
    public interface ITenantSetupService {
        Task Setup(string moniker);
    }

    public class TenantSetupService : TenantBaseService, ITenantSetupService
    {
        #region Members
        private readonly ISystemTenantsService _systemTenantService;
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly ITenantLookupItemService _tenantLookupItemService;
        private readonly ITenantUserService _tenantUserService;
        private readonly IHashingService _hashingService;
        private readonly ICosmosDbService _cosmosDbService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public TenantSetupService(ISystemTenantsService systemTenantService, ISystemLookupItemService systemLookupItemService, ITenantLookupItemService tenantLookupItemService, ITenantUserService tenantUserService, IHashingService hashingService, ICosmosDbService cosmosDbService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemTenantService = systemTenantService;
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
        public async Task Setup(string moniker)
        {
            SystemTenant systemTenant = await _systemTenantService.Get(moniker);

            DatabaseResponse databaseResponse = await _cosmosDbService.CreateDatabase(systemTenant);
            if (databaseResponse.StatusCode != HttpStatusCode.Created && databaseResponse.StatusCode != HttpStatusCode.OK) throw new TenantSetupDatabaseCouldNotBeCreatedException(moniker);

            //  Retrieve tenant containers to create.
            var containers = _configuration.GetSection("tenant:setup:containers:create").Get<List<DatabaseContainer>>(); //  from tenant-setup.json
            if (!containers.Any()) throw new TenantSetupNoContainersFoundToCreateException();

            //  Retrieve lookup groups to omit.
            var omitLookupGroups = _configuration.GetSection("tenant:setup:lookupGroups:omit").GetChildren().Select(x => x.Value).ToList(); //  from tenant-setup.json

            foreach (DatabaseContainer container in containers)
            {
                ContainerResponse containerResponse = await _cosmosDbService.CreateContainer(databaseResponse.Database, container.Name, container.PartitionKey);
                if (containerResponse.StatusCode != HttpStatusCode.Created && containerResponse.StatusCode != HttpStatusCode.OK) throw new TenantSetupCreateContainerFailedException(container.Name);

                if (container.CloneItems)
                {
                    //  Populate lookup items from system database.
                    List<LookupGroupEntity> lookupGroups = (await _systemLookupItemService.GetItems()).ToList();
                    foreach (LookupGroupEntity group in lookupGroups)
                    {
                        //  Skip over groups to ommit.
                        if (omitLookupGroups.Contains(group.Group)) continue;

                        //  Overwrite existing ID with new GUID unique to tenant.
                        group.Items.ToList().ForEach(x => x.Id = Guid.NewGuid().ToString());

                        //  Persist item.
                        await _tenantLookupItemService.CreateItem(group);
                    }
                }
            }

            //  Retrieve tenant users to create.
            var users = _configuration.GetSection("tenant:setup:users").Get<List<Entities.User>>(); //  from tenant-setup.json

            //  Persist tenant users.
            foreach (Entities.User user in users)
            {
                user.Id = Guid.NewGuid().ToString();
                user.EmployeeID = await _tenantUserService.GetUniqueEmployeeId(moniker);
                user.Username = user.Username.Replace("{moniker}", moniker);
                user.Password = _hashingService.EncryptString(user.Password);

                //  Complete each email address.
                foreach (EmailAddress emailAddress in user.EmailAddresses)
                {
                    emailAddress.Address = emailAddress.Address.Replace("{moniker}", moniker);
                    emailAddress.Type = await _tenantLookupItemService.GetItem("Email Address Types", emailAddress.Type.Name);
                }

                //  Complete each phone number.
                foreach (PhoneNumber phoneNumber in user.PhoneNumbers)
                {
                    phoneNumber.Type = await _tenantLookupItemService.GetItem("Phone Number Types", phoneNumber.Type.Name);
                }

                //  Persist item.
                await _tenantUserService.CreateItem(user);
            }
        }
        #endregion Public methods
    }
}