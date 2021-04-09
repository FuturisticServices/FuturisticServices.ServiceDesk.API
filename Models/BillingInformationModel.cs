using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TangledServices.ServicePortal.API.Models
{
    public class BillingInformationModel
    {
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string CardSecurityValue { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public AddressModel Address { get; set; }
    }
}
