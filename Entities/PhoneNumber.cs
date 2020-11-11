using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using FuturisticServices.ServiceDesk.API.Models;
using FuturisticServices.ServiceDesk.API.Common;

namespace FuturisticServices.ServiceDesk.API.Entities
{
    /// <summary>
    /// Physical phone number.
    /// </summary>
    public class PhoneNumber : EntityBase
    {
        public PhoneNumber() { }

        public PhoneNumber(PhoneNumberModel model, LookupItem type)
        {
            Id = Guid.NewGuid().ToString();
            Type = type;
            Number = model.Number;
        }

        /// <summary>
        /// Purpose/usage of the phone number.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required]
        public LookupItem Type { get; set; }

        /// <summary>
        /// The physical phone number.
        /// </summary>
        [JsonProperty(PropertyName = "number", Required = Required.Always)]
        [Required, MaxLength(11), DisplayFormat(DataFormatString = "{0:###-###-####}"), DataType(DataType.PhoneNumber)]
        public string Number { get; set; }
    }
}
