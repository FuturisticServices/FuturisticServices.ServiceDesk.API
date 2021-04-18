using System;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Entities
{
    /// <summary>
    /// A subscription is the offering a tenant signs up for.
    /// A subscription can determine the funcationality of the system.
    /// </summary>
    public class TenantSubscription : BaseEntity
    {
        public TenantSubscription() { }

        public TenantSubscription(TenantSubscriptionModel model)
        {
            Id = model.Id;
            Subscription = new Subscription(model.Subscription);
            StartDatetime = model.StartDatetime;
            EndDatetime = model.StartDatetime.AddYears(1);
            RenewalDate = EndDatetime.AddDays(1);
            IsActive = model.IsActive;
            BillingAndPaymentTermsAgreementDate = model.BillingAndPaymentTermsAgreementDate;
            ServiceTermsAgreementDate = model.ServiceTermsAgreementDate;
        }

        /// <summary>
        /// Name of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "subscription", Required = Required.Always)]
        [Required]
        public Subscription Subscription { get; set; }

        /// <summary>
        /// The date when the subscrition is set to expire.
        /// </summary>
        [JsonProperty(PropertyName = "startDatetime", Required = Required.Always)]
        public DateTime StartDatetime { get; set; }

        /// <summary>
        /// The date when the subscrition is set to expire.
        /// </summary>
        [JsonProperty(PropertyName = "endDatetime", Required = Required.AllowNull)]
        public DateTime EndDatetime { get; set; }

        /// <summary>
        /// The date when the subscription is set to automatically renew
        /// </summary>
        [JsonProperty(PropertyName = "renewalDate", Required = Required.AllowNull)]
        public DateTime RenewalDate { get; set; }

        /// <summary>
        /// The date when the subscription is set to automatically renew
        /// </summary>
        [JsonProperty(PropertyName = "isActive", Required = Required.Always)]
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Datetime of tenant agreeing to the 'Billing and Payment Terms' legal agreement
        /// </summary>
        [JsonProperty(PropertyName = "billingAndPaymentTermsAgreementDate", Required = Required.Always)]
        public DateTime BillingAndPaymentTermsAgreementDate { get; set; }

        /// <summary>
        /// Datetime of tenant agreeing to the 'Service Terms' legal agreement
        /// </summary>
        [JsonProperty(PropertyName = "serviceTermsAgreementDate", Required = Required.Always)]
        public DateTime ServiceTermsAgreementDate { get; set; }
    }
}
