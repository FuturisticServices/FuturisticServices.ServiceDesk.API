using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace TangledServices.ServiceDesk.API.Entities
{
    public class LookupGroupEntity: EntityBase
    {
        /// <summary>
        /// Container parition key. *** CANNOT CHANGE VALUE (used by Cosmos DB)!!!!
        /// Group this setting is associated with.
        /// </summary>
        [JsonProperty(PropertyName = "group")]
        [Required, JsonRequired, MaxLength(50), DisplayName("Group")]
        public string Group { get; set; }

        /// <summary>
        /// Friendly value to use for display.
        /// </summary>
        [JsonProperty(PropertyName = "label")]
        [Required, JsonRequired, MaxLength(50), DisplayName("Label")]
        public string Label { get; set; }

        /// <summary>
        /// List of values.
        /// </summary>
        [JsonProperty(PropertyName = "items", Required = Required.Default)]
        [DisplayName("Items")]
        public IEnumerable<LookupItemEntity> Items { get; set; }
    }

    public class LookupItemEntity : EntityBase
    {
        /// <summary>
        /// System value. *** SHOULD NOT CHANGE!!!!
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        [MaxLength(50), DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Value of the item.
        /// </summary>
        [JsonProperty(PropertyName = "abbreviation", Required = Required.Default)]
        [MaxLength(10), DisplayName("Abbreviation")]
        public string Abbreviation { get; set; }

        /// <summary>
        /// List of values.
        /// </summary>
        [JsonProperty(PropertyName = "items", Required = Required.Default)]
        [DisplayName("Items")]
        public IEnumerable<LookupItemEntity> Items { get; set; }
    }
}
