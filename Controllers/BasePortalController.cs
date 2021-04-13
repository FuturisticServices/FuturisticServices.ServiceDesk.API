using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Controllers
{
    public class BasePortalController : Controller
    {
        public ApiResponse response;

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
