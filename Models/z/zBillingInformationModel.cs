using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCE.ProjectVolume.Ticketing.API.Models
{
    public class zBillingInformationModel
    {
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string CardSecurityValue { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public zAddressModel Address { get; set; }
    }
}
