using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class CompanyModel
    {
        public CompanyModel() { }

        //  Create model from entity.
        public CompanyModel(Company entity)
        {
            Name = entity.Name;
            Address = new AddressModel(entity.Address);
            PhoneNumber = new PhoneNumberModel(entity.PhoneNumber);
            Website = new WebsiteModel(entity.Website);
        }

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        [Required, DisplayName("Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "address", Required = Required.Always)]
        [DisplayName("Address")]
        public AddressModel Address { get; set; }

        [JsonProperty(PropertyName = "phoneNumber", Required = Required.Always)]
        [DisplayName("PhoneNumber")]
        public PhoneNumberModel PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "website", Required = Required.Always)]
        [DisplayName("Website")]
        public WebsiteModel Website { get; set; }
    }
}
