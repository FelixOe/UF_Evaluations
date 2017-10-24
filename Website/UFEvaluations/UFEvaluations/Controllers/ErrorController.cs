using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UFEvaluations.Controllers
{
    public class ErrorController : BaseController
    {
        public ActionResult Index()
        {
            return View("Error");
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404; 
            return View("NotFound");
        }
    }
}