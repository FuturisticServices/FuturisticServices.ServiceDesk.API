using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TCE.ProjectVolume.Ticketing.API.Models
{
    public class zAddressModel
    {
        public zLookupItemModel Type { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public zLookupItemModel State { get; set; }
        public zLookupItemModel Country { get; set; }
        public string PostalCode { get; set; }
    }
}
