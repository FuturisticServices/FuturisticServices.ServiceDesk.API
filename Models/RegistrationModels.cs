using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Abstractions;

using FuturisticServices.ServiceDesk.API.Entities;

namespace FuturisticServices.ServiceDesk.API.Models
{
    /// <summary>
    /// Model to register a tenant.
    /// </summary>
    public class RegistrationModel
    {
        /// <summary>
        /// A unique abbreviation that identifies the tenant in the system.
        /// Included in each and every API request to identify which
        /// database the system will interact with.
        /// </summary>
        public string Moniker { get; set; }

        /// <summary>
        /// A GUID provided to the tenant upon a successful registration.
        /// If tenant does not immediately setup its service desk,
        /// this SetupToken will be used to authenticate the tenant
        /// when the service desk setup is eventually invoked.
        /// </summary>
        public string SetupToken { get; set; }

        /// <summary>
        /// The selected subscription ID.
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Contains information related to the tenant's legal business entity.
        /// </summary>
        public CompanyModel Company { get; set; }

        /// <summary>
        /// Contains information related to tenant's primary contact.
        /// </summary>
        public PointOfContactModel PointOfContact { get; set; }

        /// <summary>
        /// Contains information to charge the tenant for subscription feees.
        /// </summary>
        public BillingInformationModel BillingInformation { get; set; }
    }

    public class CompanyModel
    {
        public string Name { get; set; }
        public AddressModel Address { get; set; }
        public PhoneNumberModel PhoneNumber { get; set; }
        public WebsiteModel Website { get; set; }
    }
    public class PointOfContactModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public AddressModel Address { get; set; }
        public PhoneNumberModel PhoneNumber { get; set; }
        public EmailAddressModel EmailAddress { get; set; }
    }

    public class BillingInformationModel
    {
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string CardSecurityValue { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public AddressModel Address { get; set; }
    }


    public class AddressModel
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string StateId { get; set; }
        public string PostalCode { get; set; }
        public string CountryId { get; set; }
    }

    public class PhoneNumberModel
    {
        [Required, MinLength(10), MaxLength(10)]
        public string Number { get; set; }
    }

    public class EmailAddressModel
    {
        public string Address { get; set; }
    }

    public class WebsiteModel
    {
        [Required, MaxLength(50)]
        public string Url { get; set; }
    }

    public class RegistrationMetaDataModel
    {
        /// <summary>
        /// List of states.
        /// </summary>
        public List<LookupItem> States { get; set; }

        /// <summary>
        /// List of countries.
        /// </summary>
        public List<LookupItem> Countries { get; set; }

        /// <summary>
        /// List of subscriptions.
        /// </summary>
        public List<Subscription> Subscriptions { get; set; }
    }
}
