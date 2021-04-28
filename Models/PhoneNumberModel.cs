using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class PhoneNumberModel : BaseModel
    {
        public PhoneNumberModel() { }

        public PhoneNumberModel(PhoneNumber entity)
        {
            Number = entity.Number;
            Type = new LookupItemValueModel(entity.Type);
        }

        public static List<PhoneNumberModel> Construct(List<PhoneNumber> entities)
        {
            List<PhoneNumberModel> phoneNumbers = new List<PhoneNumberModel>();
            foreach (PhoneNumber phoneNumber in entities)
            {
                phoneNumbers.Add(new PhoneNumberModel(phoneNumber));
            }
            return phoneNumbers;
        }

        /// <summary>
        /// Associated LookupItem.PhoneNumberTypes.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required, DisplayName("Type")]
        public LookupItemValueModel Type { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        [JsonProperty(PropertyName = "number", Required = Required.Always)]
        [Required, DataType(DataType.PhoneNumber), DisplayName("Number")]
        public string Number { get; set; }
    }
}
