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

                //Get semesters to display
                List<Semester> semesterRange = StaticData.semesters.Where(p =>
                    new SemesterComparer().Compare(p.semester, GlobalVariables.CurrentSemesterLow) >= 0
                    && new SemesterComparer().Compare(p.semester, GlobalVariables.CurrentSemesterHigh) <= 0).ToList();

                List<CourseRating> courseRatings = CourseRatingRepositorySQL.Instance.listByCategoryAndSemesters(
                    Convert.ToInt32(GlobalVariables.CurrentCategory), semesterRange.Select(p => p.semester).ToArray())
                    .Where(p => p.classSize >= p.responses)
                    .ToList();

                //Filter only sections within the department (Not all ratings of a professor who had a course within that department)
                courseRatings = courseRatings.Where(p => p.courseID == course.courseID).ToList();

                List<Instructor> instructorsAll = InstructorRepositorySQL.Instance.listByCourse(course.courseID)
                    .Where(p => courseRatings.Select(u => u.instructorID).Distinct().Contains(p.instructorID))
                    .ToList();

                var instructorDeptMapping = instructorsAll.Select(p =>
                {
                    CourseRating firstRating = courseRatings.Where(u => u.instructorID == p.instructorID).FirstOrDefault();
                    var dept = StaticData.courseList.Where(t => t.courseID == firstRating.courseID).FirstOrDefault().departmentID;
                    return new
                    {
                        instructorID = p.instructorID,
                        departmentID = dept
                    };
                }).ToList();

                int totalResponses = 0;
                int totalStudents = 0;
                double averageRating = 0.0;

                List<InstructorDomain> instructors = instructorsAll.Select(p =>
                {
                    var courseRatingInstructor = courseRatings.Where(x => x.instructorID == p.instructorID);
                    var responses = courseRatingInstructor.Select(y => y.responses).Sum(z => z);
                    var students = courseRatingInstructor.Select(y => y.classSize).Sum(z => z);
                    var semesters = courseRatingInstructor.Select(v => v.semester).Distinct()
                        .OrderByDescending(t => t, new SemesterComparer());

                    totalResponses += responses;
                    totalStudents += students;
                    averageRating += courseRatingInstructor.Sum(z => (double)z.responses * z.ratings[0].averageRating);

                    return new InstructorDomain
                    {
                        instructorID = p.instructorID,
                        firstName = p.firstName,
                        lastName = p.lastName,
                        responses = responses.ToString(),
                        students = students.ToString(),
                        responseRate = ((double)responses / (double)students).ToString("p1"),
                        department = StaticData.departmentList.Where(x => x.departmentID == instructorDeptMapping.Where(a => a.instructorID == p.instructorID).FirstOrDefault().departmentID).FirstOrDefault().name,
                        lastSemester = (semesters.Count() > 0 ? semesters.FirstOrDefault() : ""),
                        rating = courseRatingInstructor.Sum(z => ((double)z.responses / (double)responses) * z.ratings[0].averageRating).ToString("#.##")
                    };
                }).ToList();

                viewModel.course = course;
                viewModel.instructors = instructors;
                viewModel.totalResponses = totalResponses.ToString("N0");
                viewModel.totalStudents = totalStudents.ToString("N0");
                viewModel.averageResponseRate = ((double)totalResponses / (double)totalStudents).ToString("p1");
                viewModel.averageRating = (averageRating / (double)totalResponses).ToString("#.##");
                viewModel.currentSemesterLow = GlobalVariables.CurrentSemesterLow.Split(' ')[1] + " " + GlobalVariables.CurrentSemesterLow.Split(' ')[0];
                viewModel.currentSemesterHigh = GlobalVariables.CurrentSemesterHigh.Split(' ')[1] + " " + GlobalVariables.CurrentSemesterHigh.Split(' ')[0];
            }
            else
                throw new HttpException(404, "Course not found!");

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
            int totalResponses = 0;
            int totalStudents = 0;
            int totalInstructors = 0;
            double averageRating = 0.0;

            List<CourseDomain> courses = courseList.Select(p =>
            {
                var courseRatingsList = courseRatings.Where(x => x.courseID == p.courseID);
                var responses = courseRatingsList.Select(y => y.responses).Sum(z => z);
                var students = courseRatingsList.Select(y => y.classSize).Sum(z => z);
                var instructors = courseRatingsList.Select(y => y.instructorID).Distinct().Count();

                totalResponses += responses;
                totalStudents += students;
                totalInstructors += instructors;
                averageRating += courseRatingsList
                        .Sum(z => z.responses * z.ratings[0].averageRating);

                return new CourseDomain
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
            viewModel.totalResponses = totalResponses.ToString("N0");
            viewModel.totalStudents = totalStudents.ToString("N0");
            viewModel.totalInstructors = totalInstructors.ToString("N0");
            viewModel.averageResponseRate = ((double)totalResponses / (double)totalStudents).ToString("p1");
            viewModel.averageRating = (averageRating / (double)totalResponses).ToString("#.##");
            viewModel.currentSemester = (GlobalVariables.CurrentSemester == "-1" ? "the past three semesters" : GlobalVariables.CurrentSemester.Split(' ')[1] + " " + GlobalVariables.CurrentSemester.Split(' ')[0]);

            return View(viewModel);
        }
    }
}