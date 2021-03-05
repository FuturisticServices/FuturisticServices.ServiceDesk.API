using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using TangledServices.ServiceDesk.API.Models;
using TangledServices.ServiceDesk.API.Common;

namespace TangledServices.ServiceDesk.API.Entities
{
    /// <summary>
    /// Physical email address.
    /// </summary>
    public class EmailAddress : EntityBase
    {
        public EmailAddress() { }

        public EmailAddress(EmailAddressModel model, LookupItemEntity type)
        {
            Id = Guid.NewGuid().ToString();
            Type = type;
            Address = model.Address;
        }

        /// <summary>
        /// Purpose/usage of the email address.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required]
        public LookupItemEntity Type { get; set; }

        /// <summary>
        /// Physical email address.
        /// </summary>
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        [Required, MaxLength(50), DataType(DataType.EmailAddress)]
        public string Address { get; set; }
    }
}
