using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TangledServices.ServicePortal.API.Models
{
    public class CompanyModel
    {
        public string Name { get; set; }
        public AddressModel Address { get; set; }
        public PhoneNumberModel PhoneNumber { get; set; }
        public WebsiteModel Website { get; set; }
    }
}
