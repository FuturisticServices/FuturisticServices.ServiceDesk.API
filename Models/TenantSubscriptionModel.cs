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
    /// <summary>
    /// A subscription is the offering a tenant signs up for.
    /// A subscription can determine the funcationality of the system.
    /// </summary>
    public class TenantSubscriptionModel : BaseModel
    {
        public TenantSubscriptionModel(Subscription entity, DateTime registrationDatetime)
        {
            if (registrationDatetime == null) registrationDatetime = DateTime.Now;

            Id = Guid.NewGuid().ToString();
            Subscription = new SubscriptionModel(entity);
            StartDatetime = registrationDatetime;
            EndDatetime = StartDatetime.AddYears(1);
            RenewalDate = EndDatetime.AddDays(1);
            BillingAndPaymentTermsAgreementDate = registrationDatetime;
            ServiceTermsAgreementDate = registrationDatetime;
        }

        /// <summary>
        /// Name of the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "subscription", Required = Required.Always)]
        [Required]
        public SubscriptionModel Subscription { get; set; }

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
