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
        public AddressCountry(AddressCountryModel model)
        {
            Id = model.Id;
            Name = model.Name;
            Abbreviation = model.Abbreviation;
        }

        public AddressCountry(LookupItemEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Abbreviation = entity.Abbreviation;
        }

        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }
}
