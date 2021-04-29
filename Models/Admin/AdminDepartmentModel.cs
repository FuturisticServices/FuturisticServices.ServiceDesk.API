using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class AdminDepartmentModel : BaseModel
    {
        public AdminDepartmentModel() { }

        public AdminDepartmentModel(AdminDepartment entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Abbreviation = entity.Abbreviation;
            SubDepartments = Construct(entity.SubDepartments);
        }

        public static List<AdminDepartmentModel> Construct(IEnumerable<AdminDepartment> entities)
        {
            List<AdminDepartmentModel> departments = new List<AdminDepartmentModel>();
            foreach (AdminDepartment department in entities)
            {
                departments.Add(new AdminDepartmentModel(department));
            }
            return departments;
        }

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
        public IEnumerable<AdminDepartmentModel> SubDepartments { get; set; }
    }
}
