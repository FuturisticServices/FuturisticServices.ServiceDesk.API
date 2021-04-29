using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TangledServices.ServicePortal.API.Entities;

namespace TangledServices.ServicePortal.API.Controllers
{
    public class BaseController : Controller
    {
        public ApiResponse response;
        public Dictionary<string, object> responseModels = new Dictionary<string, object>();

        ///// <summary>
        ///// Required for Swagger to work properly. Otherwise, not used by any API.
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
