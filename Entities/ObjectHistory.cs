using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using FuturisticServices.ServiceDesk.API.Models;
using FuturisticServices.ServiceDesk.API.Common;

namespace FuturisticServices.ServiceDesk.API.Entities
{
    /// <summary>
    /// A record/history of all actions performed to/on an object.
    /// </summary>
    public class ObjectHistory : EntityBase
    {
        public List<ObjectModification> Modifications { get; set; }
    }

    /// <summary>
    /// A specific 
    /// </summary>
    public class ObjectModification
    {
        public string UserId { get; set; }
        public DateTime DateTime { get; set; }
        public LookupItem Action { get; set; }
        public string BeforeModification { get; set; }
        public string AfterModification { get; set; }
    }
}
