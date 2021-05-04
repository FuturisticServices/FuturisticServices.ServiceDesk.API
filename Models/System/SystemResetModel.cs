using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class SystemResetModel
    {
        public SystemResetModel() { }

        /// <summary>
        /// Container name.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        [Required, DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Container name.
        /// </summary>
        [JsonProperty(PropertyName = "partitionKeyPath", Required = Required.Always)]
        [Required, DisplayName("Partition key path")]
        public string PartitionKeyPath { get; set; }

        [JsonProperty(PropertyName = "lookupItems", Required = Required.Default)]
        [DisplayName("Lookup items")]
        public List<SystemLookupItemModel> LookupItems { get; set; }

        [JsonProperty(PropertyName = "subscriptions", Required = Required.Default)]
        [DisplayName("Subscriptions")]
        public List<SubscriptionModel> Subscriptions { get; set; }

        [JsonProperty(PropertyName = "users", Required = Required.Default)]
        [DisplayName("Users")]
        public List<SystemUserModel> Users { get; set; }

        [JsonProperty(PropertyName = "departments", Required = Required.Default)]
        [DisplayName("Departments")]
        public List<SystemDepartmentModel> Departments { get; set; }
    }
}
