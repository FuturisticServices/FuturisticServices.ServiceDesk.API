using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using TangledServices.ServicePortal.API.Models;
using TangledServices.ServicePortal.API.Common;

namespace TangledServices.ServicePortal.API.Entities
{
    /// <summary>
    /// Physical email address.
    /// </summary>
    public class AdminEmailAddress
    {
        public AdminEmailAddress() { }

        public AdminEmailAddress(AdminEmailAddressModel model)
        {
            Type = new AdminLookupItemValue(model.Type);
            Address = model.Address;
        }

        public static List<AdminEmailAddress> Construct(List<AdminEmailAddressModel> model)
        {
            List<AdminEmailAddress> emailAddresses = new List<AdminEmailAddress>();
            foreach (AdminEmailAddressModel emailAddress in model)
            {
                emailAddresses.Add(new AdminEmailAddress(emailAddress));
            }
            return emailAddresses;
        }

        /// <summary>
        /// Purpose/usage of the email address.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required]
        public AdminLookupItemValue Type { get; set; }

        /// <summary>
        /// Physical email address.
        /// </summary>
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        [Required, MaxLength(50), DataType(DataType.EmailAddress)]
        public string Address { get; set; }
    }
}
