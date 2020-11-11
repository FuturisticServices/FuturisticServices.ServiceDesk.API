using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCE.ProjectVolume.Ticketing.API.Models
{
    public class zCompanyModel
    {
        public string Name { get; set; }
        public zAddressModel Address { get; set; }
        public zPhoneNumberModel PhoneNumber { get; set; }
        public zWebsite Website { get; set; }
    }
}
