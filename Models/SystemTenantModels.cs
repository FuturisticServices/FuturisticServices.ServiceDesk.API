using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace TangledServices.ServicePortal.API.Models
{
    /// <summary>
    /// Model to register a tenant.
    /// </summary>
    public class SystemTenantModel : BaseModel
    {
        /// <summary>
        /// A unique abbreviation that identifies the tenant in the system.
        /// Included in each and every API request to identify which
        /// database the system will interact with.
        /// </summary>
        [JsonProperty(PropertyName = "moniker", Required = Required.Always)]
        [Required, DisplayName("Moniker")]
        public string Moniker { get; set; }

        /// <summary>
        /// The selected subscription ID.
        /// </summary>
        [JsonProperty(PropertyName = "subscriptionId", Required = Required.Always)]
        [Required, DisplayName("Subscription ID")]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Contains information related to the tenant's legal business entity.
        /// </summary>
        [JsonProperty(PropertyName = "company", Required = Required.Always)]
        [Required, DisplayName("Company")]
        public CompanyModel Company { get; set; }

        /// <summary>
        /// Contains information related to tenant's primary contact.
        /// </summary>
        [JsonProperty(PropertyName = "pointOfContact", Required = Required.Always)]
        [Required, DisplayName("Point of Contact")]
        public PointOfContactModel PointOfContact { get; set; }

        /// <summary>
        /// Contains information to charge the tenant for subscription feees.
        /// </summary>
        [JsonProperty(PropertyName = "billingInformation", Required = Required.Always)]
        [Required, DisplayName("Billing information")]
        public BillingInformationModel BillingInformation { get; set; }
    }
}
