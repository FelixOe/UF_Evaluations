using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UFEvaluations.Controllers
{
    public class SettingsController : Controller
    {
        [HttpPost]
        public JsonResult SetCategory(string Category)
        {
            GlobalVariables.CurrentCategory = Category;

            return Json("{ 'message': 'success'}", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SetSemester(string Semester)
        {
            GlobalVariables.CurrentSemester = Semester;

            return Json("{ 'message': 'success'}", JsonRequestBehavior.AllowGet);
        }
    }
}