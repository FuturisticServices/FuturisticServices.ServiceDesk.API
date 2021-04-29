using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Entities
{
    public class AdminDepartment : BaseEntity
    {
        public AdminDepartment() { }

        public AdminDepartment(AdminDepartmentModel model)
        {
            Id = model.Id;
            Name = model.Name;
            Abbreviation = model.Abbreviation;
            SubDepartments = Construct(model.SubDepartments);
        }

        public static List<AdminDepartment> Construct(IEnumerable<AdminDepartmentModel> model)
        {
            List<AdminDepartment> departments = new List<AdminDepartment>();
            foreach (AdminDepartmentModel department in model)
            {
                departments.Add(new AdminDepartment(department));
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
        public IEnumerable<AdminDepartment> SubDepartments { get; set; }
    }
}
