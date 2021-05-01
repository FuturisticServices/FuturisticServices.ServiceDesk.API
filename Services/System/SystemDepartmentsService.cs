using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Managers;

namespace TangledServices.ServicePortal.API.Services
{
    public interface ISystemDepartmentsService
    {
        Task<IEnumerable<SystemDepartmentModel>> GetItems(bool flattenHierarchy = false, bool includeDeletedItems = false);
        Task<SystemDepartmentModel> GetItem(string id, bool includeSubDepartments = true, bool includeDeletedItems = false);
        Task CreateItem(SystemDepartmentModel model);
        Task<SystemDepartmentModel> UpdateItem(SystemDepartmentModel model);
        Task Delete(string id);
    }

    public class SystemDepartmentsService : SystemBaseService, ISystemDepartmentsService
    {
        private readonly ISystemDepartmentsManager _systemDepartmentsManager;
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SystemDepartmentsService(ISystemDepartmentsManager systemDepartmentsManager, IHashingService hashingService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _systemDepartmentsManager = systemDepartmentsManager;
            _hashingService = hashingService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Public methods
        public async Task CreateItem(SystemDepartmentModel model)
        {
            SystemDepartment entity = new SystemDepartment(model);
            await _systemDepartmentsManager.CreateItemAsync(entity);
        }

        /// <summary>
        /// Retrieves all system departments.
        /// </summary>
        /// <param name="flattenHierarchy">True ~ Return departments in a flattened list. False ~ Return departments in parent/child hierarchy.</param>
        /// <param name="includeDeletedItems">True ~ Include system departments that have been 'deleted'. False ~ Only include 'non-deleted' system departments.</param>
        /// <returns></returns>
        public async Task<IEnumerable<SystemDepartmentModel>> GetItems(bool flattenHierarchy = false, bool includeDeletedItems = false)
        {
            var systemDepartments = await _systemDepartmentsManager.GetItemsAsync();
            if (systemDepartments == null || !systemDepartments.Any()) throw new DepartmentsNotFoundException();

            var systemDepartmentsModel = SystemDepartmentModel.Construct(systemDepartments);    //  Utilize flattened list of system departments to search (only hit DB once).

            if (!flattenHierarchy) systemDepartments = systemDepartments.Where(x => x.ParentId == null);   // Start with parent level items.

            var model = new List<SystemDepartmentModel>();
            foreach (SystemDepartment systemDepartment in systemDepartments)
            {
                var systemDepartmentModel = new SystemDepartmentModel(systemDepartment);
                if (!flattenHierarchy) systemDepartmentModel.SubDepartments = await GetSubDepartments(systemDepartmentsModel, systemDepartmentModel, systemDepartment.Id);
                model.Add(systemDepartmentModel);
            }

            return model;
        }

        /// <summary>
        /// Retrieves ONLY the specified system department.
        /// </summary>
        /// <param name="id">System department unique identifier (GUID).</param>
        /// <param name="flattenHierarchy">True ~ Return departments in a flattened list. False ~ Return departments in parent/child hierarchy.</param>
        /// <param name="includeSubDepartments">True ~ include associated sub-departments. False ~ do NOT include associated sub-deparments.</param>
        /// <param name="includeDeletedItems">True ~ Include system departments that have been 'deleted'. False ~ Only include 'non-deleted' system departments.</param>
        /// <returns></returns>
        public async Task<SystemDepartmentModel> GetItem(string id, bool includeSubDepartments = true, bool includeDeletedItems = false)
        {
            var systemDepartments = await _systemDepartmentsManager.GetItemsAsync();
            if (systemDepartments == null || !systemDepartments.Any()) throw new DepartmentsNotFoundException();
            var systemDepartmentsModel = SystemDepartmentModel.Construct(systemDepartments);

            var systemDepartment = systemDepartments.SingleOrDefault(x => x.Id == id);
            if (systemDepartment == null || systemDepartment.IsDeleted) throw new DepartmentNotFoundException(id);

            var systemDepartmentModel = new SystemDepartmentModel(systemDepartment);
            if (includeSubDepartments) systemDepartmentModel.SubDepartments = await GetSubDepartments(systemDepartmentsModel, systemDepartmentModel, id);

            return systemDepartmentModel;
        }

        private async Task<IEnumerable<SystemDepartmentModel>> GetSubDepartments(IEnumerable<SystemDepartmentModel> systemDepartmentsModel, SystemDepartmentModel systemDepartmentModel, string id)
        {
            
            var subDepartments = systemDepartmentsModel.Where(x => string.Compare(x.ParentId, id, true) == 0);
            subDepartments = subDepartments.ToList();

            if (subDepartments.Any())
            {
                foreach (SystemDepartmentModel subDepartmentModel in subDepartments)
                {
                    subDepartmentModel.SubDepartments = await GetSubDepartments(systemDepartmentsModel, systemDepartmentModel, subDepartmentModel.Id);
                }
            }

            return subDepartments;
        }

        public async Task<SystemDepartmentModel> UpdateItem(SystemDepartmentModel model)
        {
            var department = new SystemDepartment(model);
            department = await _systemDepartmentsManager.UpdateItemAsync(department);
            var departmentModel = new SystemDepartmentModel(department);
            return departmentModel;
        }

        public async Task Delete(string id)
        {
            var department = await _systemDepartmentsManager.GetItemAsync(id);
            if (department == null) throw new DepartmentNotFoundException();

            department.IsDeleted = false;
            await _systemDepartmentsManager.UpdateItemAsync(department);
            return;
        }
        #endregion Public methods
    }
}
