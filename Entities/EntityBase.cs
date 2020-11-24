using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace FuturisticServices.ServiceDesk.API.Entities
{
    public class EntityBase
    {
        /// <summary>
        /// Unique identifier (GUID) of the entity.
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.AllowNull)]
        public string Id { get; set; }

        ///// <summary>
        ///// The partition key of the container item.
        ///// </summary>
        //[JsonProperty(PropertyName = "partitionKey", Required = Required.AllowNull)]
        //public string PartitionKey { get; set; }
    }
}
