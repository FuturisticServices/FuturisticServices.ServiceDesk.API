using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class SystemEmailAddressModel : BaseModel
    {
        public SystemEmailAddressModel() { }

        public SystemEmailAddressModel(SystemEmailAddress entity)
        {
            Address = entity.Address;
            Type = new SystemLookupItemValueModel(entity.Type);
        }

        public static List<SystemEmailAddressModel> Construct(List<SystemEmailAddress> entities)
        {
            List<SystemEmailAddressModel> emailAddresses = new List<SystemEmailAddressModel>();
            foreach (SystemEmailAddress emailAddress in entities)
            {
                emailAddresses.Add(new SystemEmailAddressModel(emailAddress));
            }
            return emailAddresses;
        }

        /// <summary>
        /// Associated LookupItem.AddressTypes.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required, DisplayName("Type")]
        public SystemLookupItemValueModel Type { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        [Required, DataType(DataType.EmailAddress), DisplayName("Address")]
        public string Address { get; set; }
    }
}
