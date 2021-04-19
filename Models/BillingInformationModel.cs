using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Models
{
    public class BillingInformationModel
    {
        public BillingInformationModel() { }

        public BillingInformationModel(BillingInformation entity)
        {
            NameOnCard = entity.NameOnCard;
            CardNumber = entity.CardNumber;
            CardSecurityValue = entity.CardSecurityValue;
            ExpirationMonth = entity.ExpirationMonth;
            ExpirationYear = entity.ExpirationYear;
            Address = new AddressModel(entity.Address);
        }

        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string CardSecurityValue { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public AddressModel Address { get; set; }
    }
}
