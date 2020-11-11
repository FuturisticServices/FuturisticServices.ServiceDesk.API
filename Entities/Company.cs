using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Newtonsoft.Json;
using FuturisticServices.ServiceDesk.API.Models;
using FuturisticServices.ServiceDesk.API.Common;

namespace FuturisticServices.ServiceDesk.API.Entities
{
    /// <summary>
    /// Information related to the legal entity of a tenant.
    /// </summary>
    public class Company : EntityBase
    {
        public Company() { }

        public Company(CompanyModel model, List<LookupGroup> systemLookupItems)
        {
            //  Get all lookup items.
            List<LookupItem> addressTypes = systemLookupItems.Where(x => x.LookupName == Enums.LookupGroups.AddressTypes.GetDescription().ToCamelCase()).SelectMany(x => x.Items).ToList();
            List<LookupItem> phoneNumberTypes = systemLookupItems.Where(x => x.LookupName == Enums.LookupGroups.PhoneNumberTypes.GetDescription().ToCamelCase()).SelectMany(x => x.Items).ToList();
            List<LookupItem> websiteTypes = systemLookupItems.Where(x => x.LookupName == Enums.LookupGroups.WebsiteTypes.GetDescription().ToCamelCase()).SelectMany(x => x.Items).ToList();
            List<LookupItem> states = systemLookupItems.Where(x => x.LookupName == Enums.LookupGroups.States.GetDescription().ToCamelCase()).SelectMany(x => x.Items).ToList();
            List<LookupItem> countries = systemLookupItems.Where(x => x.LookupName == Enums.LookupGroups.Countries.GetDescription().ToCamelCase()).SelectMany(x => x.Items).ToList();

            Id = Guid.NewGuid().ToString();
            Name = model.Name;
            Address = new Address(model.Address,
                                    addressTypes.SingleOrDefault(x => x.Name == "Corporate"),
                                    states.SingleOrDefault(x => x.Id == model.Address.StateId),
                                    countries.SingleOrDefault(x => x.Id == model.Address.CountryId));
            PhoneNumber = new PhoneNumber(model.PhoneNumber,
                                            phoneNumberTypes.SingleOrDefault(x => x.Name == "Corporate"));
            Website = new Website(model.Website,
                                    websiteTypes.SingleOrDefault(x => x.Name == "Corporate"));
        }

        /// <summary>
        /// Legal entity name of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        [Required, MaxLength(50), DisplayName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Primary address of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        [Required]
        public Address Address { get; set; }

        /// <summary>
        /// Primary phone number of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "phoneNumber", Required = Required.Always)]
        [Required]
        public PhoneNumber PhoneNumber { get; set; }

        /// <summary>
        /// Company public website URL.
        /// </summary>
        [JsonProperty(PropertyName = "website", Required = Required.Always)]
        [Required]
        public Website Website { get; set; }
    }
}
