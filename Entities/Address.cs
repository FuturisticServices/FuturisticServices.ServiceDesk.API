using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;
using FuturisticServices.ServiceDesk.API.Models;

namespace FuturisticServices.ServiceDesk.API.Entities
{
    /// <summary>
    /// Physical mailing address.
    /// </summary>
    public class Address : EntityBase
    {
        public Address() { }

        public Address(AddressModel model, LookupItem addressType, LookupItem state, LookupItem country)
        {
            Id = Guid.NewGuid().ToString();
            Type = addressType;
            Line1 = model.Line1;
            Line2 = string.IsNullOrEmpty(model.Line2) ? null : model.Line2;
            City = model.City;
            State = state;
            PostalCode = model.PostalCode;
            Country = country;
        }

        /// <summary>
        /// Purpose/usage of the address.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        [Required, JsonRequired]
        public LookupItem Type { get; set; }

        /// <summary>
        /// Line 1 associated to the address.
        /// </summary>
        [JsonProperty(PropertyName = "line1")]
        [Required, JsonRequired, MaxLength(50), DisplayName("Line 1")]
        public string Line1 { get; set; }

        /// <summary>
        /// (optional) Line 2 associated to the address.
        /// </summary>
        [JsonProperty(PropertyName = "line2")]
        [MaxLength(50), DisplayName("Line 2")]
        public string Line2 { get; set; }

        /// <summary>
        /// City associated to the address.
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        [Required, JsonRequired, MaxLength(50), DisplayName("City")]
        public string City { get; set; }

        /// <summary>
        /// State associated to the address.
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        [Required, JsonRequired, DisplayName("State")]
        public LookupItem State { get; set; }

        /// <summary>
        /// Post/zip code associated to the address.
        /// </summary>
        [JsonProperty(PropertyName = "postalCode")]
        [Required, JsonRequired, MinLength(5), MaxLength(10), DisplayName("Postal code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Country origina associated to the address.
        /// </summary>
        [JsonProperty(PropertyName = "country")]
        [Required, JsonRequired, DisplayName("Country")]
        public LookupItem Country { get; set; }
    }
}
