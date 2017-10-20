using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UFEvaluations.Controllers
{
    public class SettingsController : Controller
    {
        public ActionResult SetSettings(string DropdownCategory, string DropdownSemester)
        {
            return View();
        }
    }
}