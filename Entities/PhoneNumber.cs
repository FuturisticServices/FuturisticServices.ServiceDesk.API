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
    public class PhoneNumber
    {
        public PhoneNumber() { }

        public PhoneNumber(PhoneNumberModel model)
        {
            Type = model.Type;
            Number = model.Number;
        }

        public static List<PhoneNumber> Construct(List<PhoneNumberModel> model)
        {
            List<PhoneNumber> phoneNumbers = new List<PhoneNumber>();
            foreach (PhoneNumberModel phoneNumber in model)
            {
                phoneNumbers.Add(new PhoneNumber(phoneNumber));
            }
            return phoneNumbers;
        }

        /// <summary>
        /// Purpose/usage of the phone number.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required]
        public LookupItemEntity Type { get; set; }

        /// <summary>
        /// The physical phone number.
        /// </summary>
        [JsonProperty(PropertyName = "number", Required = Required.Always)]
        [Required, MaxLength(11), DisplayFormat(DataFormatString = "{0:###-###-####}"), DataType(DataType.PhoneNumber)]
        public string Number { get; set; }
    }
}
