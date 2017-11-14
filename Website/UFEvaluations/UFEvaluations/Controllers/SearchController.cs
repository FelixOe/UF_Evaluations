using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UFEvaluations.Models;
using UFEvaluations.Data;

namespace UFEvaluations.Controllers
{
    public class SearchController : BaseController
    {
        [HttpPost]
        public ActionResult Search(string searchInput)
        {
            if (searchInput.Length < 2)
                throw new FormatException("Argument is less than two characters long!");

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

            SearchSearchViewModel viewModel = new SearchSearchViewModel();

            List<string> instructorList = StaticData.instructorList.Select(p => GlobalFunctions.escapeQuerystringElement(p.lastName + p.firstName)).ToList();
            List<string> collegeList = StaticData.collegeList.Select(p => GlobalFunctions.escapeQuerystringElement(p.name)).ToList();
            List<string> departmentList = StaticData.departmentList.Select(p => GlobalFunctions.escapeQuerystringElement(p.name)).ToList();
            List<string> courseList = StaticData.courseList.Select(p => GlobalFunctions.escapeQuerystringElement(p.code + " - " + p.title)).ToList();

            List<Pair<string, string>> searchResults = StaticData.searchTerms.Where(p =>
                   GlobalFunctions.escapeQuerystringElement(p.First).Contains(GlobalFunctions.escapeQuerystringElement(searchInput)) ||
                   (p.Second != "" && GlobalFunctions.escapeQuerystringElement(p.Second).Contains(GlobalFunctions.escapeQuerystringElement(searchInput))))
                .Select(y =>
                {
                    string type = "";
                    string first = "";

                    if (instructorList.Contains(GlobalFunctions.escapeQuerystringElement(y.First)))
                    {
                        first = y.Second.Replace(",", "");
                        type = "Instructor";
                    }
                    else if (collegeList.Contains(GlobalFunctions.escapeQuerystringElement(y.First)))
                    {
                        first = y.First;
                        type = "College";
                    }
                    else if (departmentList.Contains(GlobalFunctions.escapeQuerystringElement(y.First)))
                    {
                        first = y.First;
                        type = "Department";
                    }
                    else if (courseList.Contains(GlobalFunctions.escapeQuerystringElement(y.First)))
                    {
                        first = y.First;
                        type = "Course";
                    }

                    return new Pair<string, string>(first, type);
                }).ToList();



            viewModel.searchTerm = searchInput;
            viewModel.searchResults = searchResults;

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult AutoCompleteSearch(string term)
        {
            if (StaticData.searchTerms != null)
            {
                var data = StaticData.searchTerms.Where(p =>
                GlobalFunctions.escapeQuerystringElement(p.First).Contains(GlobalFunctions.escapeQuerystringElement(term)) ||
                    (p.Second != "" && GlobalFunctions.escapeQuerystringElement(p.Second).Contains(GlobalFunctions.escapeQuerystringElement(term))))
                .Take(10).Select(p => p.First);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}