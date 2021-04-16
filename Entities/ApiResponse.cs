using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using TangledServices.ServicePortal.API.Extensions;

namespace TangledServices.ServicePortal.API.Entities
{
    public class ApiResponse
    {
        public ApiResponse(HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest, string description = null, string exceptionMessage = null, List<Object> responseModels = null)
        {
            StatusCode = (int)httpStatusCode;
            Status = httpStatusCode.ToString();
            if (description != null) Description = description;
            if (exceptionMessage != null) ExceptionMessage = exceptionMessage;

            if (responseModels != null)
            {
                Data = new ExpandoObject();
                List<dynamic> models = new List<dynamic>();
                foreach (Object responseModel in responseModels)
                {
                    Helpers.AddProperty(Data, responseModel.ToString().Split('.').Last(), responseModel);
                }
            }
        }

        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string ExceptionMessage { get; set; }
        public ExpandoObject Data { get; set; }
    }
    
}
