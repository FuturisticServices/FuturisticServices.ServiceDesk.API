using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class SystemDepartmentModel : BaseModel
    {
        public SystemDepartmentModel() { }

        public SystemDepartmentModel(SystemDepartment entity)
        {
            Id = entity.Id;
            ParentId = entity.ParentId;
            Name = entity.Name;
            DisplayAs = entity.DisplayAs;
            Abbreviation = entity.Abbreviation;
            SubDepartments = Construct(entity.SubDepartments);
            IsDeleted = entity.IsDeleted;
        }

        public static IEnumerable<SystemDepartmentModel> Construct(IEnumerable<SystemDepartment> entities)
        {
            if (entities == null) return null;

            List<SystemDepartmentModel> departments = new List<SystemDepartmentModel>();
            foreach (SystemDepartment department in entities)
            {
                departments.Add(new SystemDepartmentModel(department));
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
        /// Name associated to the department.
        /// </summary>
        [JsonProperty(PropertyName = "displayAs", Required = Required.Always)]
        [Required, MaxLength(50), DisplayName("Display as")]
        public string DisplayAs { get; set; }

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
        public IEnumerable<SystemDepartmentModel> SubDepartments { get; set; }

        /// <summary>
        /// If department is enabled (usable) or disabled (deleted).
        /// </summary>
        [JsonProperty(PropertyName = "isDeleted", Required = Required.Default)]
        [DefaultValue(true), DisplayName("Is deleted")]
        public bool IsDeleted { get; set; }
    }
}
