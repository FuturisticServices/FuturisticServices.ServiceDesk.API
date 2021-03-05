using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TangledServices.ServiceDesk.API.Extensions;
using TangledServices.ServiceDesk.API.Models;

namespace TangledServices.ServiceDesk.API.Entities
{
    public class Ticket : EntityBase
    {
        [JsonConstructor]
        public Ticket() { }

        public Ticket (TicketsModel model)
        {
            Id = Guid.NewGuid().ToString();
            PartitionKeyId = model.PartitionKeyId.Trim();
            Title = model.PartitionKeyId.Trim();
            Description = model.Description.Trim();
            Catalog = Extensions.Helpers.ToCamelCase(model.Catalog).Trim();
        }

        [JsonProperty(PropertyName = "partitionKeyId")]
        public string PartitionKeyId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "catalog")]
        public string Catalog { get; set; }
    }
}
