using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;
using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class AddressModel
    {
        public AddressModel() { }

        //  Create a model from and entity.
        public AddressModel(Address entity)
        {
            Type = new LookupItemValueModel(entity.Type);
            Line1 = entity.Line1;
            Line2 = entity.Line2;
            City = entity.City;
            State = new LookupItemValueModel(entity.State);
            Country = new LookupItemValueModel(entity.Country);
        }

        /// <summary>
        /// Line 1 associated to address.
        /// </summary>
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        [Required, DisplayName("Type")]
        public LookupItemValueModel Type { get; set; }

        /// <summary>
        /// Line 1 associated to address.
        /// </summary>
        [JsonProperty(PropertyName = "line1", Required = Required.Always)]
        [Required, StringLength(50, ErrorMessage = "Line 1 cannot exceed {1} characters."), DisplayName("Line 1")]
        public string Line1 { get; set; }

        /// <summary>
        /// Line 1 associated to address.
        /// </summary>
        [JsonProperty(PropertyName = "line2", Required = Required.Default)]
        [StringLength(50, ErrorMessage = "Line 2 cannot exceed {1} characters."), DisplayName("Line 2")]
        public string Line2 { get; set; }

        /// <summary>
        /// Line 1 associated to address.
        /// </summary>
        [JsonProperty(PropertyName = "city", Required = Required.Always)]
        [Required, StringLength(50, ErrorMessage = "City cannot exceed {1} characters."), DisplayName("City")]
        public string City { get; set; }

        /// <summary>
        /// Line 1 associated to address.
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        [Required, DisplayName("State")]
        public LookupItemValueModel State { get; set; }

        /// <summary>
        /// Line 1 associated to address.
        /// </summary>
        [JsonProperty(PropertyName = "postalCode", Required = Required.Always)]
        [Required, RegularExpression(@"\d{5}([ \-]\d{4})?",  ErrorMessage = "Postal code is invalid.")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Line 1 associated to address.
        /// </summary>
        [JsonProperty(PropertyName = "country")]
        [DisplayName("Country")]
        public LookupItemValueModel Country { get; set; }
    }
}
