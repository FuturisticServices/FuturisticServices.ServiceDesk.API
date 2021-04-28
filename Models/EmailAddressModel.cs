using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class EmailAddressModel : BaseModel
    {
        public EmailAddressModel() { }

        public EmailAddressModel(EmailAddress entity)
        {
            Address = entity.Address;
            Type = new LookupItemValueModel(entity.Type);
        }

        public static List<EmailAddressModel> Construct(List<EmailAddress> entities)
        {
            List<EmailAddressModel> emailAddresses = new List<EmailAddressModel>();
            foreach (EmailAddress emailAddress in entities)
            {
                emailAddresses.Add(new EmailAddressModel(emailAddress));
            }
            return emailAddresses;
        }

        /// <summary>
        /// Associated LookupItem.AddressTypes.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required, DisplayName("Type")]
        public LookupItemValueModel Type { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        [Required, DataType(DataType.EmailAddress), DisplayName("Address")]
        public string Address { get; set; }
    }
}
