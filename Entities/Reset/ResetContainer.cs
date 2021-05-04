using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace TangledServices.ServicePortal.API.Entities
{
    public class ResetContainer
    {
        public ResetContainer() { }

        public ResetContainer(string name, string partitionKey)
        {
            Name = name;
            PartitionKeyPath = partitionKey;
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
        [JsonProperty(PropertyName = "partitionKeyPath", Required = Required.Always)]
        [Required, DisplayName("Partition key path")]
        public string PartitionKeyPath { get; set; }

        [JsonProperty(PropertyName = "groups", Required = Required.Default)]
        [DisplayName("Groups")]
        public List<SystemLookupItem> Groups { get; set; }

        [JsonProperty(PropertyName = "subscriptions", Required = Required.Default)]
        [DisplayName("Subscriptions")]
        public List<Subscription> Subscriptions { get; set; }
    }
}
