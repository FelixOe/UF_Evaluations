using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UFEvaluations.Models;

namespace UFEvaluations.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            HomeViewModel viewModel = new HomeViewModel();
            viewModel.topColleges = StaticData.collegeList.Take(5).ToList();

            string currentYear = DateTime.Now.Year.ToString();
            List<CourseRating> courseRatingsCurrentYear = StaticData.overallRatingsList.Where(p => p.semester.Contains(currentYear)).ToList();

            var topcoursesList = courseRatingsCurrentYear.Select(t => t.courseID).Distinct().Select(p => new {
                courseID = p,
                enrollment = courseRatingsCurrentYear.Where(x => x.courseID == p).Select(y => y.classSize).Sum(z => z)
        }).OrderByDescending(s => s.enrollment).Take(10).ToList();

            viewModel.topCourses = topcoursesList.Select(p => StaticData.courseList.Where(x => x.courseID == p.courseID).FirstOrDefault()).ToList();
            viewModel.topDepartments = StaticData.departmentList.Take(5).ToList();
            viewModel.topInstructors = StaticData.instructorList.Take(5).ToList();

            return View(viewModel);
        }
    }
}