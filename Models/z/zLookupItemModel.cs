using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCE.ProjectVolume.Ticketing.API.Models
{
    public class zLookupItemModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Abbreviation { get; set; }
    }

    public class AddressTypes
    {
        public List<zLookupItemModel> Items { get; set; }
    }

    public class States
    {
        public List<zLookupItemModel> Items { get; set; }
    }

    public class Countries
    {
        public List<zLookupItemModel> Items { get; set; }
    }

    public class WebsiteTypes
    {
        public List<zLookupItemModel> Items { get; set; }
    }
}
