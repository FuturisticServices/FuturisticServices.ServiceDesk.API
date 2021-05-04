using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Entities
{
    public class Subscription: BaseEntity
    {
        public Subscription() { }

        //  Create entity from model.
        public Subscription(SubscriptionModel model)
        {
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
            Price = model.Price;
            IsExpired = model.IsExpired;
            PromotionCode = model.PromotionCode;
            RenewalOccurrence = model.RenewalOccurrence;
            RenewalTimeframe = model.RenewalTimeframe == null ? null : new SystemLookupItemValue(model.RenewalTimeframe);
            Highlights = model.Highlights;
        }

        //  Create entity from model.
        public Subscription(TenantSubscriptionModel model)
        {
            Id = model.Id;
            Name = model.Subscription.Name;
            Description = model.Subscription.Description;
            Price = model.Subscription.Price;
            IsExpired = model.Subscription.IsExpired;
            PromotionCode = model.Subscription.PromotionCode;
            RenewalOccurrence = model.Subscription.RenewalOccurrence;
            RenewalTimeframe = model.Subscription.RenewalTimeframe == null ? null : new SystemLookupItemValue(model.Subscription.RenewalTimeframe);
            Highlights = model.Subscription.Highlights;
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
        public SystemLookupItemValue RenewalTimeframe { get; set; }

        /// <summary>
        /// Name of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "highlights", Required = Required.AllowNull)]
        [Required, DisplayName("Highlights")]
        public IEnumerable<string> Highlights { get; set; }
    }
}
