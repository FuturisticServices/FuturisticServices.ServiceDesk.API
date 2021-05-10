using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class AdminPhoneNumberModel : BaseModel
    {
        public AdminPhoneNumberModel() { }

        public AdminPhoneNumberModel(AdminPhoneNumber entity)
        {
            Number = entity.Number;
            Type = new AdminLookupItemValueModel(entity.Type);
        }

        public AdminPhoneNumberModel(SystemPhoneNumber entity)
        {
            Number = entity.Number;
            Type = new AdminLookupItemValueModel(entity.Type);
        }

        public static List<AdminPhoneNumberModel> Construct(List<AdminPhoneNumber> entities)
        {
            List<AdminPhoneNumberModel> phoneNumbers = new List<AdminPhoneNumberModel>();
            foreach (AdminPhoneNumber phoneNumber in entities)
            {
                phoneNumbers.Add(new AdminPhoneNumberModel(phoneNumber));
            }
            return phoneNumbers;
        }

        public static List<AdminPhoneNumberModel> Construct(List<SystemPhoneNumber> entities)
        {
            List<AdminPhoneNumberModel> phoneNumbers = new List<AdminPhoneNumberModel>();
            foreach (SystemPhoneNumber phoneNumber in entities)
            {
                phoneNumbers.Add(new AdminPhoneNumberModel(phoneNumber));
            }
            return phoneNumbers;
        }

        /// <summary>
        /// Associated LookupItem.PhoneNumberTypes.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required, DisplayName("Type")]
        public AdminLookupItemValueModel Type { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        [JsonProperty(PropertyName = "number", Required = Required.Always)]
        [Required, DataType(DataType.PhoneNumber), DisplayName("Number")]
        public string Number { get; set; }
    }
}
