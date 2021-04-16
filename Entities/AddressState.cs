using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TangledServices.ServicePortal.API.Models;

namespace TangledServices.ServicePortal.API.Entities
{
    public class AddressState : BaseEntity
    {
        public AddressState() { }

        public AddressState(AddressStateModel model)
        {
            Id = model.Id;
            Name = model.Name;
            Abbreviation = model.Abbreviation;
        }

        public AddressState(LookupItemEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Abbreviation = entity.Abbreviation;
        }

        public AddressState State { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }
}
