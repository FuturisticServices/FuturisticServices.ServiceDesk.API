using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class AddressModel : BaseModel
    {
        public AddressModel() { }

        public AddressModel(Address entity)
        {
            Line1 = entity.Line1;
            Line2 = entity.Line2;
            City = entity.City;
            State = new AddressStateModel(entity.State);
            Country = new AddressCountryModel(entity.Country);
        }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public AddressStateModel State { get; set; }
        public string PostalCode { get; set; }
        public AddressCountryModel Country { get; set; }
    }
}
