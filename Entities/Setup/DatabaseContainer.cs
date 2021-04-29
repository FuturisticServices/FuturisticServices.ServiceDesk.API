using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace TangledServices.ServicePortal.API.Entities
{
    public class DatabaseContainer
    {
        public DatabaseContainer() { }

        public DatabaseContainer(string name, string partitionKeyPath, bool cloneItems)
        {
            Name = name;
            PartitionKeyPath = partitionKeyPath;
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
        [JsonProperty(PropertyName = "partitionKeyPath", Required = Required.Always)]
        [Required, DisplayName("Partition key path")]
        public string PartitionKeyPath { get; set; }

        /// <summary>
        /// Whether to clone items from the system container.
        /// </summary>
        [JsonProperty(PropertyName = "cloneItems", Required = Required.Always)]
        [Required, DisplayName("Clone items")]
        public bool CloneItems { get; set; }
    }
}
