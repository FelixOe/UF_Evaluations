using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UFEvaluations.Controllers
{
    public class SearchController : BaseController
    {
        [HttpPost]
        public ActionResult Search(string searchInput)
        {
            if (StaticData.instructorList.Select(p => GlobalFunctions.escapeQuerystringElement(p.lastName + p.firstName)).Contains(GlobalFunctions.escapeQuerystringElement(searchInput)))
            {
                Instructor thisInstructor = StaticData.instructorList
                .Where(p => GlobalFunctions.escapeQuerystringElement(p.lastName + ", " + p.firstName).Contains(GlobalFunctions.escapeQuerystringElement(searchInput))).FirstOrDefault();

                return RedirectToAction("Detail", "Instructors", new { instructor = GlobalFunctions.escapeQuerystringElement(thisInstructor.firstName + " " + thisInstructor.lastName) });
            }
            else if (StaticData.collegeList.Select(p => GlobalFunctions.escapeQuerystringElement(p.name)).Contains(GlobalFunctions.escapeQuerystringElement(searchInput)))
            {
                College thisCollege = StaticData.collegeList
                    .Where(p => GlobalFunctions.escapeQuerystringElement(p.name).Contains(GlobalFunctions.escapeQuerystringElement(searchInput))).FirstOrDefault();

                return RedirectToAction("Detail", "Colleges", new { college = GlobalFunctions.escapeQuerystringElement(thisCollege.name) });
            }
            else if (StaticData.departmentList.Select(p => GlobalFunctions.escapeQuerystringElement(p.name)).Contains(GlobalFunctions.escapeQuerystringElement(searchInput)))
            {
                Department thisDepartment = StaticData.departmentList
                    .Where(p => GlobalFunctions.escapeQuerystringElement(p.name).Contains(GlobalFunctions.escapeQuerystringElement(searchInput))).FirstOrDefault();

                return RedirectToAction("Detail", "Departments", new { department = GlobalFunctions.escapeQuerystringElement(thisDepartment.name) });
            }
            else if (StaticData.courseList.Select(p => GlobalFunctions.escapeQuerystringElement(p.code + " - " + p.title)).Contains(GlobalFunctions.escapeQuerystringElement(searchInput)))
            {
                Course thisCourse = StaticData.courseList
                    .Where(p => GlobalFunctions.escapeQuerystringElement(p.code + " - " + p.title).Contains(GlobalFunctions.escapeQuerystringElement(searchInput))).FirstOrDefault();

                return RedirectToAction("Detail", "Courses", new { course = GlobalFunctions.escapeQuerystringElement(thisCourse.code) });
            }

            return View();
        }

        [HttpPost]
        public JsonResult AutoCompleteSearch(string term)
        {
            var data = StaticData.searchTerms.Where(p => 
            GlobalFunctions.escapeQuerystringElement(p.First).Contains(GlobalFunctions.escapeQuerystringElement(term)) || 
                (p.Second != "" && GlobalFunctions.escapeQuerystringElement(p.Second).Contains(GlobalFunctions.escapeQuerystringElement(term)))).OrderBy(p => p.First.Trim())
            .Take(10).Select(p => p.First);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}