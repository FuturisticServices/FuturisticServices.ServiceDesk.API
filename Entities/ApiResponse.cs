using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace TangledServices.ServicePortal.API.Entities
{
    public class ApiResponse
    {
        public ApiResponse(HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest, string description = null, string exceptionMessage = null, Object data = null)
        {
            StatusCode = (int)httpStatusCode;
            Status = httpStatusCode.ToString();
            Description = description;
            ExceptionMessage = exceptionMessage;

            Data = new ExpandoObject();
            Data.SystemTenant = data;
        }

        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string ExceptionMessage { get; set; }
        public dynamic Data { get; set; }
    }
}
