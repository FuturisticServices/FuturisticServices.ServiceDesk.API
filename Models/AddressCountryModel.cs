using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class AddressCountryModel : BaseModel
    {
        public AddressCountryModel() { }

        public AddressCountryModel(AddressCountry entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Abbreviation = entity.Abbreviation;
        }

        public AddressCountryModel(LookupItemEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Abbreviation = entity.Abbreviation;
        }

        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }
}
