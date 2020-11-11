using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using FuturisticServices.ServiceDesk.API.Entities;

namespace FuturisticServices.ServiceDesk.API.Entities
{
    public class Subscription: EntityBase
    {
        /// <summary>
        /// Name of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        [Required, MaxLength(50), DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Name of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "description", Required = Required.AllowNull)]
        [Required, MaxLength(50), DisplayName("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Name of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "price", Required = Required.Always)]
        [Required, DisplayName("Price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Name of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "promotionCode", Required = Required.Default)]
        [MaxLength(10), DisplayName("Promotion code")]
        public string PromotionCode { get; set; }

        /// <summary>
        /// Name of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "renewalOccurence", Required = Required.Always)]
        [Required, DisplayName("Renewal occurence")]
        public int RenewalOccurrence { get; set; }

        /// <summary>
        /// Name of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "renewalTimeframe", Required = Required.AllowNull)]
        [DisplayName("Renewal timeframe")]
        public LookupItem RenewalTimeframe { get; set; }

        /// <summary>
        /// Name of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "highlights", Required = Required.AllowNull)]
        [Required, DisplayName("Highlights")]
        public IEnumerable<string> Highlights { get; set; }
    }
}
