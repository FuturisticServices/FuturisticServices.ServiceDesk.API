using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCE.ProjectVolume.Ticketing.API.Models
{
    public class zPointOfContact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public zAddressModel Address { get; set; }
        public zPhoneNumberModel PhoneNumber { get; set; }
        public zEmailAddressModel EmailAddress { get; set; }
    }
}
