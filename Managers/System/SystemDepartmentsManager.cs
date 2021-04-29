using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Managers
{
    public interface ISystemDepartmentsManager
    {
        Task<IEnumerable<SystemDepartment>> GetItemsAsync();
        Task<SystemDepartment> GetItemAsync(string id);
        Task<SystemDepartment> CreateItemAsync(SystemDepartment department);
        Task<SystemDepartment> UpdateItemAsync(SystemDepartment department);
    }

    public class SystemDepartmentsManager : SystemBaseManager, ISystemDepartmentsManager
    {
        internal IConfiguration _configuration;
        internal IWebHostEnvironment _webHostEnvironment;

        public SystemDepartmentsManager(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base("Departments", configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<SystemDepartment>> GetItemsAsync()
        {
            var query = _container.GetItemLinqQueryable<SystemDepartment>();
            var iterator = query.ToFeedIterator();
            var result = await iterator.ReadNextAsync();
            return result;
        }

        public async Task<SystemDepartment> GetItemAsync(string id)
        {
            IEnumerable<SystemDepartment> departments = await GetItemsAsync();
            if (departments == null || !departments.Any()) throw new DepartmentsNotFoundException();

            SystemDepartment systemDepartment = departments.SingleOrDefault(x => string.Compare(x.Id, id, true) == 0);
            return systemDepartment;
        }

        public async Task<SystemDepartment> CreateItemAsync(SystemDepartment department)
        {
            var results = await _container.CreateItemAsync<SystemDepartment>(department, new PartitionKey(department.Name));
            return results;
        }

        public async Task<SystemDepartment> UpdateItemAsync(SystemDepartment department)
        {
            var results = await _container.UpsertItemAsync<SystemDepartment>(department);
            return results;
        }
    }
}
