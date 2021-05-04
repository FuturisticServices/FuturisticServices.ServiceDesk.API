using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class PointOfContactModel
    {
        public PointOfContactModel() { }

        public PointOfContactModel(PointOfContact entity)
        {
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            Title = entity.Title;
            Address = new SystemAddressModel(entity.Address);
            PhoneNumber = new SystemPhoneNumberModel(entity.PhoneNumber);
            EmailAddress = new SystemEmailAddressModel(entity.EmailAddress);
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public SystemAddressModel Address { get; set; }
        public SystemPhoneNumberModel PhoneNumber { get; set; }
        public SystemEmailAddressModel EmailAddress { get; set; }
    }
}
