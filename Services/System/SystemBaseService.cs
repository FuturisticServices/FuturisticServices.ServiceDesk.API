using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

namespace TangledServices.ServiceDesk.API.Services
{
    public class SystemBaseService
    {
        public SystemBaseService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {

        }
    }
}
