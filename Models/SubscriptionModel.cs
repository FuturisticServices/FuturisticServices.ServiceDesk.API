using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class SubscriptionModel : BaseModel
    {
        public SubscriptionModel() {}

        //  Create a model from an entity.
        public SubscriptionModel(Subscription entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Description = entity.Description;
            Price = entity.Price;
            IsExpired = entity.IsExpired;
            PromotionCode = entity.PromotionCode;
            RenewalOccurrence = entity.RenewalOccurrence;
            RenewalTimeframe = new LookupItemValueModel(entity.RenewalTimeframe);
            Highlights = entity.Highlights;
        }

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

        [JsonProperty(PropertyName = "isExpired", Required = Required.Always)]
        [Required, DisplayName("Is Expired")]
        public bool IsExpired { get; set; }

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
        public LookupItemValueModel RenewalTimeframe { get; set; }

        /// <summary>
        /// Name of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "highlights", Required = Required.AllowNull)]
        [Required, DisplayName("Highlights")]
        public IEnumerable<string> Highlights { get; set; }
    }
}
