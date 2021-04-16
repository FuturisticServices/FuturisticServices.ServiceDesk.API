using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class EmailAddressModel : BaseModel
    {
        public EmailAddressModel() { }

        public EmailAddressModel(EmailAddress entity)
        {
            Id = entity.Id;
            Address = entity.Address;
        }

        public string Address { get; set; }
    }
}
