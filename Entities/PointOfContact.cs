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
            Address = new SystemAddress(model.Address);
            PhoneNumber = new SystemPhoneNumber(model.PhoneNumber);
            EmailAddress = new SystemEmailAddress(model.EmailAddress);
        }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        
        [JsonProperty(PropertyName = "address")]
        public SystemAddress Address { get; set; }
        
        /// <summary>
        /// The primary phone number associated to the PoC.
        /// </summary>
        [JsonProperty(PropertyName = "phoneNumber")]
        [Required, JsonRequired]
        public SystemPhoneNumber PhoneNumber { get; set; }
        
        /// <summary>
        /// The primary email addresses associated to PoC.
        /// </summary>
        [JsonProperty(PropertyName = "emailAddress")]
        [Required, JsonRequired]
        public SystemEmailAddress EmailAddress { get; set; }
    }
}
