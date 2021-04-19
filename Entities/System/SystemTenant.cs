using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Entities
{
    public class SystemTenant : BaseEntity
    {
        public SystemTenant() { }

        //  Create entity from model.
        public SystemTenant(SystemTenantModel model)
        {
            Id = model.Id;
            Moniker = model.Moniker;
            Subscription = new Subscription(model.Subscription);
            Company = new Company(model.Company);
            PointOfContact = new PointOfContact(model.PointOfContact);
            BillingInformation = new BillingInformation(model.BillingInformation);
        }

        /// <summary>
        /// Tenant unique identifiers used when invoking APIs.
        /// </summary>
        [JsonProperty(PropertyName = "moniker", Required = Required.Always)]
        [Required, MinLength(3), MaxLength(10), DisplayName("Moniker")]
        public string Moniker { get; set; }

        /// <summary>
        /// Legal name of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "company", Required = Required.Always)]
        [Required]
        public Company Company { get; set; }

        /// <summary>
        /// Tenant point of contact.
        /// </summary>
        [JsonProperty(PropertyName = "pointOfContact", Required = Required.Always)]
        [Required]
        public PointOfContact PointOfContact { get; set; }

        /// <summary>
        /// Tenant payment information for registration and subscription renewals.
        /// </summary>
        [JsonProperty(PropertyName = "billingInformation", Required = Required.Always)]
        [Required]
        public BillingInformation BillingInformation { get; set; }

        ///// <summary>
        ///// Tenant active subscription currently in use.
        ///// </summary>
        //[JsonProperty(PropertyName = "currentSubscription", Required = Required.AllowNull)]
        //public TenantSubscription CurrentSubscription { get; set; }

        /// <summary>
        /// Tenant active subscription currently in use.
        /// </summary>
        [JsonProperty(PropertyName = "subscription", Required = Required.AllowNull)]
        public Subscription Subscription { get; set; }

        /// <summary>
        /// All tenant subscriptions including active and expired.
        /// </summary>
        ///
        [JsonProperty(PropertyName = "subscriptions", Required = Required.AllowNull)]
        public virtual List<Subscription> Subscriptions { get; set; }
    }
}
