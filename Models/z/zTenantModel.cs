using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TCE.ProjectVolume.Ticketing.API.Entities;

namespace TCE.ProjectVolume.Ticketing.API.Models
{
    public class zTenantModel
    {
        [JsonConstructor]
        public zTenantModel() { }

        public zTenantModel(Tenant tenant)
        {
            Company = tenant.Company;
            PointOfContact = tenant.PointOfContact;
            TenantSubscription = tenant.TenantSubscriptions.SingleOrDefault(x=>x.isActive == true);
            Moniker = tenant.Moniker;
        }

        public zCompanyModel Company { get; set; }
        public Entities.PointOfContact PointOfContact { get; set; }
        public TenantSubscription TenantSubscription { get; set; }
        public string Moniker { get; set; }
    }
}
