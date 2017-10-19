using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UFEvaluations.Models;

namespace UFEvaluations.Controllers
{
    public class BaseController : Controller
    {
        public LayoutViewModel model;

        public BaseController()
        {
            model = new LayoutViewModel();

            model.searchScript = GlobalFunctions.createAutoCompleteScript();

            ViewBag.LayoutViewModel = model;
        }
    }
}