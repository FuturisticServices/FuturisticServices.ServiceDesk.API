using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TangledServices.ServicePortal.API.Models
{
    public class PointOfContactModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public AddressModel Address { get; set; }
        public PhoneNumberModel PhoneNumber { get; set; }
        public EmailAddressModel EmailAddress { get; set; }
    }
}
