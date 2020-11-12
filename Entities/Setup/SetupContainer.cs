using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace FuturisticServices.ServiceDesk.API.Entities
{
    public class SetupContainer
    {
        public SetupContainer() { }

        public SetupContainer(string name, string partitionKey)
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
    }
}
