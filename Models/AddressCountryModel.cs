using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;
using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class AddressCountryModel : BaseModel
    {
        public AddressCountryModel() { }

        //  Create model from entity.
        public AddressCountryModel(AddressCountry entity)
        {
            Id = entity.Id;
            Type = entity.Type;
        }

        /// <summary>
        /// Associated LookupItemEntity object of country.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        [Required, DisplayName("Type")]
        public LookupItemEntity Type { get; set; }
    }
}
