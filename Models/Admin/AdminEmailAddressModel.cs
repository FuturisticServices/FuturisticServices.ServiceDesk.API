using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class AdminEmailAddressModel : BaseModel
    {
        public AdminEmailAddressModel() { }

        public AdminEmailAddressModel(AdminEmailAddress entity)
        {
            Address = entity.Address;
            Type = new AdminLookupItemValueModel(entity.Type);
        }

        public AdminEmailAddressModel(SystemEmailAddress entity)
        {
            Address = entity.Address;
            Type = new AdminLookupItemValueModel(entity.Type);
        }

        public static List<AdminEmailAddressModel> Construct(List<AdminEmailAddress> entities)
        {
            List<AdminEmailAddressModel> emailAddresses = new List<AdminEmailAddressModel>();
            foreach (AdminEmailAddress emailAddress in entities)
            {
                emailAddresses.Add(new AdminEmailAddressModel(emailAddress));
            }
            return emailAddresses;
        }

        public static List<AdminEmailAddressModel> Construct(List<SystemEmailAddress> entities)
        {
            List<AdminEmailAddressModel> emailAddresses = new List<AdminEmailAddressModel>();
            foreach (SystemEmailAddress emailAddress in entities)
            {
                emailAddresses.Add(new AdminEmailAddressModel(emailAddress));
            }
            return emailAddresses;
        }

        /// <summary>
        /// Associated LookupItem.AddressTypes.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required, DisplayName("Type")]
        public AdminLookupItemValueModel Type { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        [Required, DataType(DataType.EmailAddress), DisplayName("Address")]
        public string Address { get; set; }
    }
}
