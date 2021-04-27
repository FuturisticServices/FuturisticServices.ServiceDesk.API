using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace TangledServices.ServicePortal.API.Entities
{
    public abstract class BaseEntity
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        [DisplayName("ID")]
        public string Id { get; set; }
    }
}
