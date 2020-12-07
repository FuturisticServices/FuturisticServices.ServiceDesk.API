using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FuturisticServices.ServiceDesk.API.Models;
using FuturisticServices.ServiceDesk.API.Common;
using FuturisticServices.ServiceDesk.API.Entities;

namespace FuturisticServices.ServiceDesk.API.Entities
{
    /// <summary>
    /// Contains information how the tenant will pay for registration and
    /// subscription renewals.
    /// </summary>
    public class BillingInformation : EntityBase
    {
        public BillingInformation() { }

        public BillingInformation(BillingInformationModel model, List<LookupGroup> systemLookupItems)
        {
            //  Get all lookup items.
            List<LookupItem> addressTypes = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.AddressTypes.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
            List<LookupItem> states = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.States.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
            List<LookupItem> countries = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.Countries.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();

            Id = Guid.NewGuid().ToString();
            NameOnCard = model.NameOnCard;
            CardNumber = model.CardNumber;
            CardSecurityValue = model.CardSecurityValue;
            ExpirationMonth = model.ExpirationMonth;
            ExpirationYear = model.ExpirationYear;
            Address = new Address(model.Address,
                                    addressTypes.SingleOrDefault(x => x.Name == "Billing"),
                                    states.SingleOrDefault(x => x.Id == model.Address.StateId),
                                    countries.SingleOrDefault(x => x.Id == model.Address.CountryId));
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
        public Address Address { get; set; }
    }
}
