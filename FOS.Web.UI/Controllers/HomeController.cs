using FOS.Setup;
using FOS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FOS.DataLayer;
using FluentValidation.Results;
using FOS.Web.UI.Models;
using FOS.Web.UI.Common.CustomAttributes;



namespace FOS.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        [CustomAuthorize]
        public ActionResult UserHome()
        {
            return View();
        }
        public ActionResult AuditComponent()
        {
            return View();
        }
    }
}
