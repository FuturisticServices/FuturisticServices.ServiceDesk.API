using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Common;
using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Entities
{
    public class PointOfContact
    {
        public PointOfContact() { }

        //  Create entity from model.
        public PointOfContact(PointOfContactModel model)
        {
            FirstName = model.FirstName;
            LastName = model.LastName;
            Title = model.Title;
            Address = new Address(model.Address);
            PhoneNumber = new PhoneNumber(model.PhoneNumber);
            EmailAddress = new EmailAddress(model.EmailAddress);
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
