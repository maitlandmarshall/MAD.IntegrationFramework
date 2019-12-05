using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Services.Internals.Http.Controllers
{
    public sealed class StatusController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
