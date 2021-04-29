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
        Task<IEnumerable<SystemDepartmentModel>> GetItems(bool includeDeletedItems = false);
        Task<SystemDepartment> GetItem(string id);
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

        public async Task<IEnumerable<SystemDepartmentModel>> GetItems(bool includeDeletedItems = false)
        {
            var departments = await _systemDepartmentsManager.GetItemsAsync();
            if (!includeDeletedItems) departments = departments.Where(x => x.IsDeleted == true);

            var departmentModel = SystemDepartmentModel.Construct(departments);
            return departmentModel;
        }

        public async Task<SystemDepartment> GetItem(string id)
        {
            var department = await _systemDepartmentsManager.GetItemAsync(id);
            if (department == null) throw new DepartmentNotFoundException(id);

            return department;
        }

        public async Task<SystemDepartmentModel> UpdateItem(SystemDepartmentModel model)
        {
            var department = await GetItem(model.Id);
            if (department == null) throw new DepartmentNotFoundException(model.Name);

            department = new SystemDepartment(model);
            department = await _systemDepartmentsManager.UpdateItemAsync(department);
            var departmentModel = new SystemDepartmentModel(department);
            return departmentModel;
        }

        public async Task Delete(string id)
        {
            var department = await GetItem(id);
            if (department == null) throw new DepartmentNotFoundException();

            department.IsDeleted = false;
            await _systemDepartmentsManager.UpdateItemAsync(department);
            return;
        }
        #endregion Public methods
    }
}
