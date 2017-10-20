using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UFEvaluations.Models;

namespace UFEvaluations.Controllers
{
    public class CoursesController : BaseController
    {
        public ViewResult Detail()
        {
            CourseDetailViewModel viewModel = new CourseDetailViewModel();

            if (Request.QueryString["course"] != null && StaticData.courseList.Where(p => GlobalFunctions.escapeQuerystringElement(p.code) == GlobalFunctions.escapeQuerystringElement(Request.QueryString["course"].ToString())).Count() == 1)
            {
                Course course = StaticData.courseList
                    .Where(p => GlobalFunctions.escapeQuerystringElement(p.code) == GlobalFunctions.escapeQuerystringElement(Request.QueryString["course"].ToString()))
                    .FirstOrDefault();

                List<CourseRating> courseRatings = CourseRatingRepositorySQL.Instance.listByCategoryAndSemesters(
                    Convert.ToInt32(GlobalVariables.CurrentCategory),
                    (GlobalVariables.CurrentSemester == "-1" ? StaticData.semesters.Take(3).Select(y => y.semester).ToArray() : new[] { GlobalVariables.CurrentSemester }))
                    .Where(p => p.classSize >= p.responses)
                    .ToList();

                //Filter only sections within the department (Not all ratings of a professor who had a course within that department)
                courseRatings = courseRatings.Where(p => p.courseID == course.courseID).ToList();

                List<Instructor> instructors = InstructorRepositorySQL.Instance.listByCourse(course.courseID)
                    .Where(p => courseRatings.Select(u => u.instructorID).Distinct().Contains(p.instructorID))
                    .ToList();

                var instructorDeptMapping = instructors.Select(p =>
                {
                    CourseRating firstRating = courseRatings.Where(u => u.instructorID == p.instructorID).FirstOrDefault();
                    var dept = StaticData.courseList.Where(t => t.courseID == firstRating.courseID).FirstOrDefault().departmentID;
                    return new
                    {
                        instructorID = p.instructorID,
                        departmentID = dept
                    };
                }).ToList();

                instructors = instructors.Select(p =>
                {
                    var courseRatingInstructor = courseRatings.Where(x => x.instructorID == p.instructorID);
                    var responses = courseRatingInstructor.Select(y => y.responses).Sum(z => z);
                    var students = courseRatingInstructor.Select(y => y.classSize).Sum(z => z);
                    var semesters = courseRatingInstructor.Select(v => v.semester).Distinct()
                        .OrderByDescending(t => t, new SemesterComparer());
                    return new Instructor
                    {
                        instructorID = p.instructorID,
                        firstName = p.firstName,
                        lastName = p.lastName,
                        responses = responses.ToString(),
                        students = students.ToString(),
                        responseRate = ((double)responses / (double)students).ToString("p1"),
                        //TODO: Retrieve all departments for each instructor
                        department = StaticData.departmentList.Where(x => x.departmentID == instructorDeptMapping.Where(a => a.instructorID == p.instructorID).FirstOrDefault().departmentID).FirstOrDefault().name,
                        lastSemester = (semesters.Count() > 0 ? semesters.FirstOrDefault() : ""),
                        rating = courseRatingInstructor.Sum(z => ((double)z.responses / (double)responses) * z.ratings[0].averageRating).ToString("#.##")
                    };
                }).ToList();

                viewModel.course = course;
                viewModel.instructors = instructors;
            }

            return View(viewModel);
        }

        public ActionResult List()
        {
            CourseListViewModel viewModel = new CourseListViewModel();

            List<CourseRating> courseRatings = CourseRatingRepositorySQL.Instance.listByCategoryAndSemesters(
                Convert.ToInt32(GlobalVariables.CurrentCategory),
                (GlobalVariables.CurrentSemester == "-1" ? StaticData.semesters.Take(3).Select(y => y.semester).ToArray() : new[] { GlobalVariables.CurrentSemester }))
                .Where(p => p.classSize >= p.responses)
                .ToList();

            List<int> courseIDs = courseRatings.Select(y => y.courseID).Distinct().ToList();

            var courseList = StaticData.courseList.Where(p => courseIDs.Contains(p.courseID));

            List<Course> courses = courseList.Select(p =>
            {
                var courseRatingsList = courseRatings.Where(x => x.courseID == p.courseID);
                var responses = courseRatingsList.Select(y => y.responses).Sum(z => z);
                var students = courseRatingsList.Select(y => y.classSize).Sum(z => z);
                var instructors = courseRatingsList.Select(y => y.instructorID).Distinct().Count();
                return new Course
                {
                    code = p.code,
                    title = p.title,
                    departmentID = p.departmentID,
                    departmentName = StaticData.departmentList.Where(u => u.departmentID == p.departmentID).FirstOrDefault().name,
                    rating = courseRatingsList
                        .Sum(z => ((double)z.responses / (double)responses) * z.ratings[0].averageRating).ToString("#.##"),
                    responses = responses.ToString(),
                    instructors = instructors.ToString(),
                    students = students.ToString(),
                    responseRate = ((double)responses / (double)students).ToString("p1")
                };
            }).Where(t => Convert.ToInt32(t.students) > 0).ToList();

            viewModel.courses = courses;

            return View(viewModel);
        }
    }
}