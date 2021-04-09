using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Services
{
    public class SystemBaseService
    {
        internal ApiResponse _response;

        public SystemBaseService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        { }
    }
}
