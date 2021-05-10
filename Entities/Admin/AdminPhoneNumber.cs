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
    /// Physical phone number.
    /// </summary>
    public class AdminPhoneNumber
    {
        public AdminPhoneNumber() { }

        public AdminPhoneNumber(AdminPhoneNumberModel model)
        {
            Type = new AdminLookupItemValue(model.Type);
            Number = model.Number;
        }

        public static List<AdminPhoneNumber> Construct(List<AdminPhoneNumberModel> model)
        {
            List<AdminPhoneNumber> phoneNumbers = new List<AdminPhoneNumber>();
            foreach (AdminPhoneNumberModel phoneNumber in model)
            {
                phoneNumbers.Add(new AdminPhoneNumber(phoneNumber));
            }
            return phoneNumbers;
        }

        /// <summary>
        /// Purpose/usage of the phone number.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required]
        public AdminLookupItemValue Type { get; set; }

        /// <summary>
        /// The physical phone number.
        /// </summary>
        [JsonProperty(PropertyName = "number", Required = Required.Always)]
        [Required, MaxLength(11), DisplayFormat(DataFormatString = "{0:###-###-####}"), DataType(DataType.PhoneNumber)]
        public string Number { get; set; }
    }
}
