using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class PointOfContactModel : BaseModel
    {
        public PointOfContactModel() { }

        public PointOfContactModel(PointOfContact entity)
        {
            Id = entity.Id;
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            Title = entity.Title;
            Address = new AddressModel(entity.Address);
            PhoneNumber = new PhoneNumberModel(entity.PhoneNumber);
            EmailAddress = new EmailAddressModel(entity.EmailAddress);
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public AddressModel Address { get; set; }
        public PhoneNumberModel PhoneNumber { get; set; }
        public EmailAddressModel EmailAddress { get; set; }
    }
}
