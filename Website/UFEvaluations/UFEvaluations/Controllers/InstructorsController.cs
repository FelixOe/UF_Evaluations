using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UFEvaluations.Models;

namespace UFEvaluations.Controllers
{
    public class InstructorsController : BaseController
    {
        public ViewResult Detail()
        {
            InstructorDetailViewModel viewModel = new InstructorDetailViewModel();

            if (Request.QueryString["instructor"] != null && StaticData.instructorList.Where(p => GlobalFunctions.escapeQuerystringElement(p.firstName + p.lastName) == GlobalFunctions.escapeQuerystringElement(Request.QueryString["instructor"].ToString())).Count() == 1)
            {
                Instructor instructor = StaticData.instructorList
                    .Where(p => GlobalFunctions.escapeQuerystringElement(p.firstName + p.lastName) == GlobalFunctions.escapeQuerystringElement(Request.QueryString["instructor"].ToString()))
                    .FirstOrDefault();

                //Get semesters to display
                List<Semester> semesters = StaticData.semesters.Where(p =>
                    new SemesterComparer().Compare(p.semester, GlobalVariables.CurrentSemesterLow) >= 0
                    && new SemesterComparer().Compare(p.semester, GlobalVariables.CurrentSemesterHigh) <= 0).ToList();

                List<CourseRating> courseRatingsOverall = CourseRatingRepositorySQL.Instance.listByInstructor(instructor.instructorID);

                //Get all course ratings for the instructor, and adjust to the semester range
                List<CourseRating> courseRatings = courseRatingsOverall
                    .Where(p => semesters.Select(y => y.semester).Contains(p.semester)).ToList();

                //Calculate number of responses and number of students
                var responsesOverall = courseRatingsOverall.Select(y => y.responses).Sum(z => z);
                var studentsOverall = courseRatingsOverall.Select(y => y.classSize).Sum(z => z);
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
                            .Sum(z => ((double)z.responses / (double)courseResponses) * z.ratings.Where(a => a.categoryID == Convert.ToInt32(GlobalVariables.CurrentCategory)).FirstOrDefault().averageRating).ToString("#.##"),
                        responses = courseResponses.ToString(),
                        students = courseStudents.ToString(),
                        responseRate = ((double)courseResponses / (double)courseStudents).ToString("p1")
                    };
                }).Where(t => Convert.ToInt32(t.students) > 0).ToList();

                //Populate graph
                InstructorGraph instructorGraph = new InstructorGraph();

                //Create a list of IEnumerables, in which each element will hold a pair, first for the semester, to sort by, second for the rating
                List<IEnumerable<Pair<string, string>>> graphData = new List<IEnumerable<Pair<string, string>>>();
                //List of labels to display on the graph, in this case the category names
                List<string> labels = new List<string>();

                //Loop through all categories and calculate the instructor rating for each semester for said category
                foreach (var category in StaticData.categoryList.Where(p => p.name != "NULL"))
                {
                    graphData.Add(courseRatings.Select(p => p.semester).Distinct().Select(u =>
                    {
                        var graphRatingsList = courseRatings.Where(x => x.semester == u);
                        var graphResponses = graphRatingsList.Select(y => y.responses).Sum(z => z);

                        return new Pair<string, string>(u,
                            graphRatingsList
                                .Sum(z => ((double)z.responses / (double)graphResponses) * z.ratings.Where(e => e.categoryID == category.categoryID).FirstOrDefault().averageRating).ToString("#.##")
                        );
                    }).OrderBy(p => p.First, new SemesterComparer())
                    );
                    labels.Add(category.name);
                }

                //Join all semesters once for the labels to be in JSON
                instructorGraph.labels = "['" + String.Join("', '", graphData[0].Select(p => p.First).ToArray()) + "']";

                instructorGraph.data = new List<string>();

                //Join the data for each linegraph to be in JSON
                foreach (var line in graphData)
                    instructorGraph.data.Add("[" + String.Join(", ", line.Select(p => p.Second).ToArray()) + "]");

                viewModel.instructor = instructor;
                viewModel.overallRatings = overallRatings;
                viewModel.courseRatingsAll = courseRatings.Select(p => 
                {
                    Course thisCourse = StaticData.courseList.Where(y => y.courseID == p.courseID).FirstOrDefault();
                    p.courseCode = thisCourse.code;
                    return p;
                }).ToList();
                viewModel.firstTerm = courseRatingsOverall.Select(v => v.semester).Distinct()
                    .OrderBy(t => t, new SemesterComparer()).FirstOrDefault().ToString();
                viewModel.responsesAll = responsesOverall.ToString();
                viewModel.studentsAll = studentsOverall.ToString();
                viewModel.departments = departments;
                viewModel.responseRateOverall = ((double)responsesOverall / (double)studentsOverall).ToString("p1");
                viewModel.courses = courses;
                viewModel.currentCategory = StaticData.categoryList.Where(p => p.categoryID == Convert.ToInt32(GlobalVariables.CurrentCategory))
                    .FirstOrDefault().name;
                ViewBag.loadChart = GlobalFunctions.createMultipleChartScript(instructorGraph, labels);
            }

            return View(viewModel);
        }

        public ActionResult List()
        {
            InstructorListViewModel viewModel = new InstructorListViewModel();

            List<CourseRating> courseRatings = CourseRatingRepositorySQL.Instance.listByCategoryAndSemesters(
                Convert.ToInt32(GlobalVariables.CurrentCategory),
                (GlobalVariables.CurrentSemester == "-1" ? StaticData.semesters.Take(3).Select(y => y.semester).ToArray() : new[] { GlobalVariables.CurrentSemester }))
                .Where(p => p.classSize >= p.responses)
                .ToList();

            List<int> instructorIDs = courseRatings.Select(y => y.instructorID).Distinct().ToList();

            var instructorList = StaticData.instructorList.Where(p => instructorIDs.Contains(p.instructorID));

            List<Instructor> instructors = instructorList.Select(p =>
            {
                var courseRatingsList = courseRatings.Where(x => x.instructorID == p.instructorID);
                var responses = courseRatingsList.Select(y => y.responses).Sum(z => z);
                var students = courseRatingsList.Select(y => y.classSize).Sum(z => z);
                var semesters = courseRatingsList.Select(v => v.semester).Distinct()
                    .OrderByDescending(t => t, new SemesterComparer());
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