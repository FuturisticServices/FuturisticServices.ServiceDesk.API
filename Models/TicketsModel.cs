using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuturisticServices.ServiceDesk.API.Models
{
    public class TicketsModel
    {
        public string Id { get; set; }
        public string PartitionKeyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Catalog { get; set; }
    }
}
