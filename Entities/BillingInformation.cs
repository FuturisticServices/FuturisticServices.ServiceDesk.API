using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Entities
{
    /// <summary>
    /// Contains information how the tenant will pay for registration and
    /// subscription renewals.
    /// </summary>
    public class BillingInformation : BaseEntity
    {
        public BillingInformation() { }

        //  Create entity from model.
        public BillingInformation(BillingInformationModel model)
        {
            Id = Guid.NewGuid().ToString();
            NameOnCard = model.NameOnCard;
            CardNumber = model.CardNumber;
            CardSecurityValue = model.CardSecurityValue;
            ExpirationMonth = model.ExpirationMonth;
            ExpirationYear = model.ExpirationYear;
            Address = new SystemAddress(model.Address);
        }

        /// <summary>
        /// Full name as it appears on the credit card.
        /// </summary>
        public string NameOnCard { get; set; }

        /// <summary>
        /// Full credit card number.
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// The credit card security value (CSV).
        /// </summary>
        public string CardSecurityValue { get; set; }

        /// <summary>
        /// The two (2) digit credit card expiration month.
        /// </summary>
        public int ExpirationMonth { get; set; }

        /// <summary>
        /// The two (2) digit credit card expiration year.
        /// </summary>
        public int ExpirationYear { get; set; }

        /// <summary>
        /// The billing address of the credit card.
        /// </summary>
        public SystemAddress Address { get; set; }
    }
}
