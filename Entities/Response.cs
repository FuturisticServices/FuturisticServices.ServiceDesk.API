using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TangledServices.ServicePortal.API.Entities
{
    public class ResponseSuccess
    {
        public int StatusCode { get; set; }
        public string Description { get; set; }
    }

    public class ResponseError
    {
        public int StatusCode { get; set; }
        public string Description { get; set; }
    }
}
