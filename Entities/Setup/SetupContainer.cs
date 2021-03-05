using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace TangledServices.ServiceDesk.API.Entities
{
    public class SetupContainer
    {
        public SetupContainer() { }

        public SetupContainer(string name, string partitionKey, bool cloneItems)
        {
            Name = name;
            PartitionKey = partitionKey;
            CloneItems = cloneItems;
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

        /// <summary>
        /// Whether to clone items from the system container.
        /// </summary>
        [JsonProperty(PropertyName = "cloneItems", Required = Required.Always)]
        [Required, DisplayName("Clone items")]
        public bool CloneItems { get; set; }
    }
}
