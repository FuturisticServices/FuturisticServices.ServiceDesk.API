using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Entities
{
    public class AddressCountry : BaseEntity
    {
        public AddressCountry() { }

        //  Create entity from model.
        public AddressCountry(AddressCountryModel model)
        {
            Id = model.Id;
            Type = model.Type;
        }

        /// <summary>
        /// Associated LookupItemEntity object of country.
        /// </summary>
        public LookupItemEntity Type { get; set; }
    }
}
