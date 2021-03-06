﻿using System;
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
        public ApiResponse(HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest, string message = null, Dictionary<string, Object> responseModels = null)
        {
            StatusCode = (int)httpStatusCode;
            Status = httpStatusCode.ToString();
            Message = message;

            if (responseModels != null)
            {
                Data = new ExpandoObject();
                List<dynamic> models = new List<dynamic>();
                foreach (var responseModel in responseModels)
                {
                    Helpers.AddProperty(Data, responseModel.Key, responseModel.Value);
                }
            }
        }

        public ApiResponse(HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest, string message = null, Exception exception = null, Dictionary<string, Object> responseModels = null)
        {
            StatusCode = (int)httpStatusCode;
            Status = httpStatusCode.ToString();
            Message = message;
            if (exception != null) ExceptionMessage = exception.Message;
            if (exception != null) InnerException = exception.InnerException;

            if (responseModels != null)
            {
                Data = new ExpandoObject();
                List<dynamic> models = new List<dynamic>();
                foreach (var responseModel in responseModels)
                {
                    Helpers.AddProperty(Data, responseModel.Key, responseModel.Value);
                }
            }
        }

        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public Exception InnerException { get; set; }
        public ExpandoObject Data { get; set; }
    }
    
}
