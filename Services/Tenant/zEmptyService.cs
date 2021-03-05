using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using TangledServices.ServiceDesk.API.Common;
using TangledServices.ServiceDesk.API.Entities;
using TangledServices.ServiceDesk.API.Managers;
using TangledServices.ServiceDesk.API.Models;
using TangledServices.ServiceDesk.API.Services;

namespace TangledServices.ServiceDesk.API.Services
{
    public interface IzEmptyService
    {

    }

    public class zEmptyService : TenantBaseService, IzEmptyService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public zEmptyService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
    }
}