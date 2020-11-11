using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCE.ProjectVolume.Ticketing.API.Models
{
    public class zSubscriptionModel
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string PromotionCode { get; set; }
        public int RenewalOccurrence { get; set; }
        public zLookupItemModel RenewalTimeframe { get; set; }
        public List<zLookupItemModel> Highlights { get; set; }
    }
}
