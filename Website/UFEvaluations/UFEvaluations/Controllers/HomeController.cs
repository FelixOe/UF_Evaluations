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

            List<CourseRating> courseRatings = StaticData.overallRatingsList
                .Where(p => StaticData.termsToDisplay.Contains(p.semester) && p.classSize >= p.responses)
                .ToList();


            //Get top 10 courses by enrollment
            var topcoursesList = courseRatings.Select(t => t.courseID).Distinct().Select(p => new {
                courseID = p,
                enrollment = courseRatings.Where(x => x.courseID == p).Select(y => y.classSize).Sum(z => z)
                }).OrderByDescending(s => s.enrollment).Take(10).ToList();

            viewModel.topCourses = topcoursesList.Select(p => StaticData.courseList.Where(x => x.courseID == p.courseID).FirstOrDefault())
                .Select(x => new KeyValuePair<string, string>(x.code + " - " + x.title, topcoursesList.Where(y => y.courseID == x.courseID).FirstOrDefault().enrollment.ToString()))
                .ToList();


            //Get top 20 instructors by overall rating
            var topinstructorsList = courseRatings.Select(t => t.instructorID).Distinct().Select(p =>
            {
                var responses = courseRatings.Where(x => x.instructorID == p).Select(y => y.responses).Sum(z => z);
                return new
                {
                    instructorID = p,
                    responses = responses,
                    rating = courseRatings.Where(x => x.instructorID == p).Sum(z => ((double)z.responses / (double)responses) * z.ratings[0].averageRating),
                };
            }).Where(u => u.responses > 50).OrderByDescending(s => s.rating).Take(20).ToList();
            
            viewModel.topInstructors = topinstructorsList.Select(p => StaticData.instructorList.Where(x => x.instructorID == p.instructorID).FirstOrDefault())
                .Select(x => new KeyValuePair<string, string>(x.firstName + " " + x.lastName, topinstructorsList.Where(y => y.instructorID == x.instructorID).FirstOrDefault().rating.ToString("#.##")))
                .ToList();


            //Get top 20 departments by overall rating
            var courseRatingsDept = courseRatings.Join(StaticData.courseList, prim => prim.courseID, fore => fore.courseID,
                (prim, fore) => new { fore.departmentID, prim.classSize, prim.responses, prim.ratings });

            var departmentList = StaticData.departmentList.Select(t => t.departmentID).Select(p =>
            {
                var responses = courseRatingsDept.Where(x => x.departmentID == p).Select(y => y.responses).Sum(z => z);
                return new
                {
                    departmentID = p,
                    rating = courseRatingsDept
                    .Where(x => x.departmentID == p).Sum(z => ((double)z.responses / (double)responses) * z.ratings[0].averageRating),
                    responses = responses,
                    classSizes = courseRatingsDept.Where(x => x.departmentID == p).Select(y => y.classSize).Sum(z => z),
                    collegeID = StaticData.departmentList.Where(a => a.departmentID == p).FirstOrDefault().collegeID
                };
            }).ToList();

            var topdepartmentsList = departmentList.Where(u => u.responses > 100).OrderByDescending(s => s.rating).Take(20).ToList();

            viewModel.topDepartments = topdepartmentsList.Select(p => StaticData.departmentList.Where(x => x.departmentID == p.departmentID).FirstOrDefault())
                .Select(x => new KeyValuePair<string, string>(x.name, topdepartmentsList.Where(y => y.departmentID == x.departmentID).FirstOrDefault().rating.ToString("#.##")))
                .ToList();


            //Get top 20 colleges by overall rating
            var courseRatingsCol = courseRatingsDept.Join(StaticData.departmentList, prim => prim.departmentID, fore => fore.departmentID,
                (prim, fore) => new { fore.collegeID, prim.classSize, prim.responses, prim.ratings });

            var topcollegesList = StaticData.collegeList.Select(t => t.collegeID).Select(p =>
            {
                var responses = courseRatingsCol.Where(x => x.collegeID == p).Select(y => y.responses).Sum(z => z);
                return new
                {
                    collegeID = p,
                    rating = courseRatingsCol
                    .Where(x => x.collegeID == p).Sum(z => ((double)z.responses / (double)responses) * z.ratings[0].averageRating),
                    responses = responses,
                };
            }).Where(u => u.responses > 100).OrderByDescending(s => s.rating).Take(20).ToList();

            viewModel.topColleges = topcollegesList.Select(p => StaticData.collegeList.Where(x => x.collegeID == p.collegeID).FirstOrDefault())
                .Select(x => new KeyValuePair<string, string>(x.name, topcollegesList.Where(y => y.collegeID == x.collegeID).FirstOrDefault().rating.ToString("#.##")))
                .ToList();

            return View(viewModel);
        }
    }
}