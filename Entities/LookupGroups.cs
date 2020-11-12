using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace FuturisticServices.ServiceDesk.API.Entities
{
    public class LookupGroup: EntityBase
    {
        /// <summary>
        /// Container parition key. *** CANNOT CHANGE VALUE (used by Cosmos DB)!!!!
        /// Group this setting is associated with.
        /// </summary>
        [JsonProperty(PropertyName = "group")]
        [Required, JsonRequired, MaxLength(50), DisplayName("Group")]
        public string Group { get; set; }

        /// <summary>
        /// System value. *** SHOULD NOT CHANGE!!!!
        /// </summary>
        [JsonProperty(PropertyName = "lookupName")]
        [Required, JsonRequired, MaxLength(50), DisplayName("Lookup name")]
        public string LookupName { get; set; }

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
        public IEnumerable<LookupItem> Items { get; set; }

        /// <summary>
        /// List of values.
        /// </summary>
        [JsonProperty(PropertyName = "subscriptions", Required = Required.Default)]
        [DisplayName("Subscriptions")]
        public IEnumerable<Subscription> Subscriptions { get; set; }
    }

    public class LookupItem : EntityBase
    {
        /// <summary>
        /// System value. *** SHOULD NOT CHANGE!!!!
        /// </summary>
        [JsonProperty(PropertyName = "lookupName", Required = Required.Default)]
        [MaxLength(50), DisplayName("Lookup name")]
        public string LookupName { get; set; }

        /// <summary>
        /// Value of the item.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        [Required, MaxLength(50), DisplayName("Name")]
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
        public IEnumerable<LookupItem> Items { get; set; }
    }
}
