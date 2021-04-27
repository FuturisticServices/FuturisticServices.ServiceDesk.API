using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class CustomerAdminModels
    {
        public  class Setup
        {
            public class Container
            {
                public Container() { }

                public Container(string name, string partitionKey)
                {
                    Name = name;
                    PartitionKey = partitionKey;
                }

                /// <summary>
                /// Container name.
                /// </summary>
                [JsonProperty(PropertyName = "name", Required = Required.Always)]
                [Required, DisplayName("Name")]
                public string Name { get; set; }

                /// <summary>
                /// Container name.
                /// </summary>
                [JsonProperty(PropertyName = "partitionKey", Required = Required.Always)]
                [Required, DisplayName("Partition key")]
                public string PartitionKey { get; set; }

                [JsonProperty(PropertyName = "groups", Required = Required.Default)]
                [DisplayName("Groups")]
                public List<LookupGroup> Groups { get; set; }

                [JsonProperty(PropertyName = "users", Required = Required.Default)]
                [DisplayName("Users")]
                public List<Entities.User> Users { get; set; }

                [JsonProperty(PropertyName = "tenants", Required = Required.Default)]
                [DisplayName("Tenants")]
                public List<Entities.Tenant> Tenants { get; set; }
            }
        }        
    }
}
