using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json;

using TangledServices.ServiceDesk.API.Models;
using TangledServices.ServiceDesk.API.Common;
using TangledServices.ServiceDesk.API.Entities;

namespace TangledServices.ServiceDesk.API.Entities
{
    public class PointOfContact : EntityBase
    {
        public PointOfContact() { }

        public PointOfContact(PointOfContactModel model, List<LookupGroupEntity> systemLookupItems)
        {
            //  Get all lookup items.
            List<LookupItemEntity> addressTypes = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.AddressTypes.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
            List<LookupItemEntity> phoneNumberTypes = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.PhoneNumberTypes.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
            List<LookupItemEntity> emailAddressTypes = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.EmailAddressTypes.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
            List<LookupItemEntity> states = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.States.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();
            List<LookupItemEntity> countries = systemLookupItems.Where(x => x.Group == Enums.LookupGroups.Countries.GetDescription().ToTitleCase()).SelectMany(x => x.Items).ToList();

            Id = Guid.NewGuid().ToString();
            FirstName = model.FirstName;
            LastName = model.LastName;
            Title = model.Title;
            Address = new Address(model.Address,
                                    addressTypes.SingleOrDefault(x => x.Name == "Work"),
                                    states.SingleOrDefault(x => x.Id == model.Address.StateId),
                                    countries.SingleOrDefault(x => x.Id == model.Address.CountryId));
            PhoneNumber = new PhoneNumber(model.PhoneNumber, phoneNumberTypes.SingleOrDefault(x => x.Name == "Work"));
            EmailAddress = new EmailAddress(model.EmailAddress, emailAddressTypes.SingleOrDefault(x => x.Name == "Work"));
        }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        
        [JsonProperty(PropertyName = "address")]
        public Address Address { get; set; }
        
        /// <summary>
        /// The primary phone number associated to the PoC.
        /// </summary>
        [JsonProperty(PropertyName = "phoneNumber")]
        [Required, JsonRequired]
        public PhoneNumber PhoneNumber { get; set; }
        
        /// <summary>
        /// The primary email addresses associated to PoC.
        /// </summary>
        [JsonProperty(PropertyName = "emailAddress")]
        [Required, JsonRequired]
        public EmailAddress EmailAddress { get; set; }
    }
}
