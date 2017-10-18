using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UFEvaluations.Models;

namespace UFEvaluations.Controllers
{
    public class InstructorsController : Controller
    {
        public ViewResult Detail()
        {
            InstructorDetailViewModel viewModel = new InstructorDetailViewModel();

            if (Request.QueryString["instructor"] != null && StaticData.instructorList.Where(p => GlobalFunctions.escapeQuerystringElement(p.firstName + p.lastName) == GlobalFunctions.escapeQuerystringElement(Request.QueryString["instructor"].ToString())).Count() == 1)
            {
                Instructor instructor = StaticData.instructorList
                    .Where(p => GlobalFunctions.escapeQuerystringElement(p.firstName + p.lastName) == GlobalFunctions.escapeQuerystringElement(Request.QueryString["instructor"].ToString()))
                    .FirstOrDefault();

                List<CourseRating> courseRatings = CourseRatingRepositorySQL.Instance.listByInstructor(instructor.instructorID);

                var responses = courseRatings.Select(y => y.responses).Sum(z => z);
                var students = courseRatings.Select(y => y.classSize).Sum(z => z);
                List<string> departments = new List<string>();

                List<OverallRating> overallRatings = StaticData.categoryList.Where(p => p.name != "NULL").Select(p => {
                    return new OverallRating {
                        category = p.name,
                        rating = courseRatings.Sum(z => ((double)z.responses / (double)responses) * z.ratings.Where(a => a.categoryID == p.categoryID).FirstOrDefault().averageRating).ToString("#.##")
                    };
                }).ToList();

                List<Course> courses = StaticData.courseList.Where(p => courseRatings.Select(a => a.courseID).Distinct().Contains(p.courseID)).Select(p => {
                    var courseRatingsList = courseRatings.Where(x => x.courseID == p.courseID);
                    var courseResponses = courseRatingsList.Select(y => y.responses).Sum(z => z);
                    var courseStudents = courseRatingsList.Select(y => y.classSize).Sum(z => z);
                    var instructors = courseRatingsList.Select(y => y.instructorID).Distinct().Count();
                    var deptName = StaticData.departmentList.Where(u => u.departmentID == p.departmentID).FirstOrDefault().name;
                    if(!departments.Contains(deptName))
                        departments.Add(deptName);

                    return new Course
                    {
                        code = p.code,
                        title = p.title,
                        departmentID = p.departmentID,
                        departmentName = deptName,
                        rating = courseRatingsList
                            .Sum(z => ((double)z.responses / (double)courseResponses) * z.ratings[0].averageRating).ToString("#.##"),
                        responses = courseResponses.ToString(),
                        students = courseStudents.ToString(),
                        responseRate = ((double)courseResponses / (double)courseStudents).ToString("p1")
                    };
                }).Where(t => Convert.ToInt32(t.students) > 0).ToList();

                //Populate graph
                InstructorGraph instructorGraph = new InstructorGraph();

                var data = courseRatings.Select(p => p.semester).Distinct().Select(u => {
                    var graphRatingsList = courseRatings.Where(x => x.semester == u);
                    var graphResponses = graphRatingsList.Select(y => y.responses).Sum(z => z);

                    return new {
                        semester = u,
                        rating = graphRatingsList
                            .Sum(z => ((double)z.responses / (double)graphResponses) * z.ratings.Where(e => e.categoryID == 10).FirstOrDefault().averageRating).ToString("#.##")
                    };
                }).OrderBy(p => p.semester);

                instructorGraph.labels = "['" + String.Join("', '", data.Select(p => p.semester).ToArray()) + "']";
                instructorGraph.data = "[" + String.Join(", ", data.Select(p => p.rating).ToArray()) + "]";

                viewModel.instructor = instructor;
                viewModel.overallRatings = overallRatings;
                viewModel.courseRatingsAll = courseRatings;
                viewModel.firstTerm = courseRatings.Select(v => Convert.ToInt32(v.semester.Split(' ')[0])).Distinct()
                    .OrderBy(t => t).FirstOrDefault().ToString();
                viewModel.responsesAll = responses.ToString();
                viewModel.studentsAll = students.ToString();
                viewModel.departments = departments;
                viewModel.responseRateOverall = ((double)responses / (double)students).ToString("p1");
                viewModel.courses = courses;
                ViewBag.loadChart = GlobalFunctions.createChartScript(instructorGraph);

            }

            return View(viewModel);
        }

        public ActionResult List()
        {
            InstructorListViewModel viewModel = new InstructorListViewModel();

            List<CourseRating> courseRatings = StaticData.overallRatingsList
                .Where(p => StaticData.termsToDisplay.Contains(p.semester) && p.classSize >= p.responses)
                .ToList();

            List<int> instructorIDs = courseRatings.Select(y => y.instructorID).Distinct().ToList();

            var instructorList = StaticData.instructorList.Where(p => instructorIDs.Contains(p.instructorID));

            List<Instructor> instructors = instructorList.Select(p =>
            {
                var courseRatingsList = courseRatings.Where(x => x.instructorID == p.instructorID);
                var responses = courseRatingsList.Select(y => y.responses).Sum(z => z);
                var students = courseRatingsList.Select(y => y.classSize).Sum(z => z);
                var semesters = courseRatingsList.Select(v =>
                {
                    //TODO: Get correct order b/w Fall, Spring, and Summer
                    return new
                    {
                        year = Convert.ToInt32(v.semester.Split(' ')[0]),
                        semester = v.semester.Split(' ')[1].ToString()
                    };
                }).Distinct().OrderByDescending(t => t.year).Select(u => u.semester + " " + u.year);
                return new Instructor
                {
                    instructorID = p.instructorID,
                    firstName = p.firstName,
                    lastName = p.lastName,
                    lastSemester = (semesters.Count() > 0 ? semesters.FirstOrDefault() : ""),
                    rating = courseRatingsList
                        .Sum(z => ((double)z.responses / (double)responses) * z.ratings[0].averageRating).ToString("#.##"),
                    responses = responses.ToString(),
                    students = students.ToString(),
                    responseRate = ((double)responses / (double)students).ToString("p1")
                };
            }).Where(t => Convert.ToInt32(t.students) > 0).ToList();

            viewModel.instructors = instructors;

            return View(viewModel);
        }
    }
}