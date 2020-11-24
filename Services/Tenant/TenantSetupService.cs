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

using FuturisticServices.ServiceDesk.API.Common;
using FuturisticServices.ServiceDesk.API.Entities;
using FuturisticServices.ServiceDesk.API.Managers;
using FuturisticServices.ServiceDesk.API.Models;
using FuturisticServices.ServiceDesk.API.Services;

namespace FuturisticServices.ServiceDesk.API.Services
{
    public interface ITenantSetupService {
        Task<IDictionary<string, Object>> ServiceDesk(string moniker);
    }

    public class TenantSetupService : TenantBaseService, ITenantSetupService
    {
        #region Members
        private readonly ISystemTenantService _systemTenantService;
        private readonly ISystemLookupItemService _systemLookupItemService;
        private readonly ITenantLookupItemService _tenantLookupItemService;
        private readonly ITenantUserService _tenantUserService;
        private readonly ICosmosDbService _cosmosDbService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        #endregion Members

        #region Constructors
        public TenantSetupService(ISystemTenantService systemTenantService, ISystemLookupItemService systemLookupItemService, ITenantLookupItemService tenantLookupItemService, ITenantUserService tenantUserService, ICosmosDbService cosmosDbService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemTenantService = systemTenantService;
            _systemLookupItemService = systemLookupItemService;
            _tenantLookupItemService = tenantLookupItemService;
            _tenantUserService = tenantUserService;
            _cosmosDbService = cosmosDbService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion Constructors

        #region Public methods
        public async Task<IDictionary<string, Object>> ServiceDesk(string moniker)
        {
            var responseContainers = new ExpandoObject() as IDictionary<string, object>;
            var responseUsers = new ExpandoObject() as IDictionary<string, object>;

            dynamic response = new ExpandoObject();
            response.status = new ExpandoObject();
            response.results = new ExpandoObject();
            response.results.databases = new ExpandoObject();
            response.results.containers = responseContainers;
            response.results.users = responseUsers;

            Tenant tenant = await _systemTenantService.GetItem(moniker);

            if (tenant == null) throw new TenantDoesNotExistException(moniker);

            DatabaseResponse databaseResponse = await _cosmosDbService.CreateDatabase(tenant);

            //  Add database response.
            response.results.databases = new { name = databaseResponse.Resource.Id, statusCode = databaseResponse.StatusCode, statusCodeValue = databaseResponse.StatusCode.GetDescription() };

            if (databaseResponse.StatusCode == HttpStatusCode.Created || databaseResponse.StatusCode == HttpStatusCode.OK)
            {
                //  Retrieve tenant containers to create.
                var containersToCreate = _configuration.GetSection("tenant:setup:containers:create").Get<List<SetupContainer>>();

                //  Retrieve lookup groups to omit.
                var lookupGroupsToOmit = _configuration.GetSection("tenant:setup:lookupGroups:omit").GetChildren().Select(x => x.Value).ToList();

                foreach (SetupContainer container in containersToCreate)
                {
                    ContainerResponse containerResponse = await _cosmosDbService.CreateContainer(databaseResponse.Database, container.Name, container.PartitionKey);

                    //  Add container response
                    responseContainers.Add(container.Name, new { statusCode = containerResponse.StatusCode, statusCodeValue = containerResponse.StatusCode.GetDescription() });

                    if (container.CloneItems == true)
                    {
                        // Populate lookup items from system database.
                        List<LookupGroup> lookupGroups = (await _systemLookupItemService.GetItems()).ToList();
                        foreach (LookupGroup group in lookupGroups)
                        {
                            //  Skip over groups to ommit.
                            if (lookupGroupsToOmit.Contains(group.Name)) continue;

                            //  Overwrite existing ID with new GUID unique to tenant.
                            group.Items.ToList().ForEach(x => x.Id = Guid.NewGuid().ToString());

                            //  Persist item.
                            await _tenantLookupItemService.CreateItem(group);

                            //  Add group response.
                            responseContainers.Add(group.Name, new { statusCode = StatusCodes.Status201Created, statusCodeValue = HttpStatusCode.Created.GetDescription(), items = group.Items.ToList() });
                        }
                    }
                }

                //  Retrieve tenant users to create.
                var users = _configuration.GetSection("tenant:setup:users").Get<List<Entities.User>>();

                // Persist tenant users.
                foreach (Entities.User user in users)
                {
                    //  Create unique user ID.
                    user.Id = Guid.NewGuid().ToString();

                    //  Persist item.
                    await _tenantUserService.CreateItem(user);

                    //  Add status to response.
                    responseUsers.Add(string.Format("user: {0} {1}", user.NameFirst, user.NameLast), new { statusCode = StatusCodes.Status201Created, statusCodeValue = HttpStatusCode.Created.GetDescription() });
                }

                response.status = new { statusCode = StatusCodes.Status201Created, statusCodeValue = HttpStatusCode.Created.GetDescription() };
                return response;
            }

            response.status = new { statusCode = StatusCodes.Status400BadRequest, statusCodeValue = HttpStatusCode.BadRequest.GetDescription() };
            return response;
        }
        #endregion Public methods
    }
}