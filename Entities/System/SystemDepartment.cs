using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Entities
{
    public class SystemDepartment : BaseEntity
    {
        public SystemDepartment() { }

        public SystemDepartment(SystemDepartmentModel model)
        {
            Id = string.IsNullOrWhiteSpace(model.Id) ? Guid.NewGuid().ToString() : model.Id;
            ParentId = model.ParentId;
            Name = model.Name;
            Abbreviation = model.Abbreviation;
            SubDepartments = Construct(model.SubDepartments);
            IsDeleted = model.IsDeleted;
        }

        public static List<SystemDepartment> Construct(IEnumerable<SystemDepartmentModel> model)
        {
            if (model == null) return null;

            List<SystemDepartment> departments = new List<SystemDepartment>();
            foreach (SystemDepartmentModel department in model)
            {
                departments.Add(new SystemDepartment(department));
            }
            return departments;
        }

        /// <summary>
        /// Parent ID of a sub-department (Parent department = null).
        /// </summary>
        [JsonProperty(PropertyName = "parentId", Required = Required.Default)]
        [DefaultValue(null), DisplayName("Parent ID")]
        public string ParentId { get; set; }

        /// <summary>
        /// Name associated to the department.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        [Required, MaxLength(50), DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Abbreviation associated to the department.
        /// </summary>
        [JsonProperty(PropertyName = "abbreviation", Required = Required.Always)]
        [Required, MinLength(2), MaxLength(10), DisplayName("Abbreviation")]
        public string Abbreviation { get; set; }

        /// <summary>
        /// Sub departments associated to the parent department.
        /// </summary>
        [JsonProperty(PropertyName = "subDepartments", Required = Required.Default)]
        [DisplayName("SubDepartments")]
        public IEnumerable<SystemDepartment> SubDepartments { get; set; }

        /// <summary>
        /// If department is enabled (usable) or disabled (deleted).
        /// </summary>
        [JsonProperty(PropertyName = "isDeleted", Required = Required.Default)]
        [DefaultValue(true), DisplayName("Is deleted")]
        public bool IsDeleted { get; set; }
    }
}
