using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class SystemPhoneNumberModel : BaseModel
    {
        public SystemPhoneNumberModel() { }

        public SystemPhoneNumberModel(SystemPhoneNumber entity)
        {
            Number = entity.Number;
            Type = new SystemLookupItemValueModel(entity.Type);
        }

        public static List<SystemPhoneNumberModel> Construct(List<SystemPhoneNumber> entities)
        {
            List<SystemPhoneNumberModel> phoneNumbers = new List<SystemPhoneNumberModel>();
            foreach (SystemPhoneNumber phoneNumber in entities)
            {
                phoneNumbers.Add(new SystemPhoneNumberModel(phoneNumber));
            }
            return phoneNumbers;
        }

        /// <summary>
        /// Associated LookupItem.PhoneNumberTypes.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required, DisplayName("Type")]
        public SystemLookupItemValueModel Type { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        [JsonProperty(PropertyName = "number", Required = Required.Always)]
        [Required, DataType(DataType.PhoneNumber), DisplayName("Number")]
        public string Number { get; set; }
    }
}
