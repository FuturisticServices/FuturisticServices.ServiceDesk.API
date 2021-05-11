using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace TangledServices.ServicePortal.API.Entities
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// A unique GUID identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        [Required, DisplayName("ID")]
        public string Id { get; set; }

        /// <summary>
        /// True/false if this item has been marked as 'deleted' and non-usable except for historical purposes.
        /// </summary>
        [JsonProperty(PropertyName = "enabled", Required = Required.Always)]
        [Required, DefaultValue(true), DisplayName("Enabled")]
        public bool Enabled { get; set; } = true;
    }
}
