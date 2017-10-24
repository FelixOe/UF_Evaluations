using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UFEvaluations.Models;

namespace UFEvaluations.Controllers
{
    public class DepartmentsController : BaseController
    {
        public ViewResult Detail()
        {
            DepartmentDetailViewModel viewModel = new DepartmentDetailViewModel();

            if (Request.QueryString["department"] != null && StaticData.departmentList.Where(p => GlobalFunctions.escapeQuerystringElement(p.name) == GlobalFunctions.escapeQuerystringElement(Request.QueryString["department"].ToString())).Count() == 1)
            {
                Department department = StaticData.departmentList
                    .Where(p => GlobalFunctions.escapeQuerystringElement(p.name) == GlobalFunctions.escapeQuerystringElement(Request.QueryString["department"].ToString()))
                    .FirstOrDefault();

                List<CourseRating> courseRatings = CourseRatingRepositorySQL.Instance.listByCategoryAndSemesters(
                    Convert.ToInt32(GlobalVariables.CurrentCategory),
                    (GlobalVariables.CurrentSemester == "-1" ? StaticData.semesters.Take(3).Select(y => y.semester).ToArray() : new[] { GlobalVariables.CurrentSemester }))
                    .Where(p => p.classSize >= p.responses)
                    .ToList();

                //Filter only sections within the department (Not all ratings of a professor who had a course within that department)
                courseRatings = courseRatings.Where(p => StaticData.courseDeptMapping[p.courseID.ToString()] == department.departmentID).ToList();

                List<Instructor> instructors = InstructorRepositorySQL.Instance.listByDepartment(department.departmentID)
                    .Where(p => courseRatings.Select(u => u.instructorID).Distinct().Contains(p.instructorID))
                    .ToList();

                var instructorDeptMapping = instructors.Select(p => {
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

                instructors = instructors.Select(p => {
                    var courseRatingInstructor = courseRatings.Where(x => x.instructorID == p.instructorID);
                    var responses = courseRatingInstructor.Select(y => y.responses).Sum(z => z);
                    var students = courseRatingInstructor.Select(y => y.classSize).Sum(z => z);
                    var semesters = courseRatingInstructor.Select(v => v.semester).Distinct()
                        .OrderByDescending(t => t, new SemesterComparer());

                    totalResponses += responses;
                    totalStudents += students;
                    averageRating += courseRatingInstructor.Sum(z => (double)z.responses * z.ratings[0].averageRating);

                    return new Instructor
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

                viewModel.department = department;
                viewModel.instructors = instructors;
                viewModel.totalResponses = totalResponses.ToString("N0");
                viewModel.totalStudents = totalStudents.ToString("N0");
                viewModel.averageResponseRate = ((double)totalResponses / (double)totalStudents).ToString("p1");
                viewModel.averageRating = (averageRating / (double)totalResponses).ToString("#.##");
                viewModel.currentSemester = (GlobalVariables.CurrentSemester == "-1" ? "the past three semesters" : GlobalVariables.CurrentSemester.Split(' ')[1] + " " + GlobalVariables.CurrentSemester.Split(' ')[0]);
            }

            return View(viewModel);
        }

        public ActionResult List()
        {
            DepartmentListViewModel viewModel = new DepartmentListViewModel();

            List<CourseRating> courseRatings = CourseRatingRepositorySQL.Instance.listByCategoryAndSemesters(
                Convert.ToInt32(GlobalVariables.CurrentCategory),
                (GlobalVariables.CurrentSemester == "-1" ? StaticData.semesters.Take(3).Select(y => y.semester).ToArray() : new[] { GlobalVariables.CurrentSemester }))
                .Where(p => p.classSize >= p.responses)
                .ToList();

            var courseRatingsDept = courseRatings.Select(p => {
                var result = new {
                    departmentID = StaticData.courseDeptMapping[p.courseID.ToString()],
                    classSize = p.classSize,
                    responses = p.responses,
                    ratings = p.ratings
                };

                return result;
            });

            int totalResponses = 0;
            int totalStudents = 0;
            double averageRating = 0.0;

            List<Department> departments = StaticData.departmentList.Select(p =>
            {
                var responses = courseRatingsDept.Where(x => x.departmentID == p.departmentID).Select(y => y.responses).Sum(z => z);
                var students = courseRatingsDept.Where(x => x.departmentID == p.departmentID).Select(y => y.classSize).Sum(z => z);

                totalResponses += responses;
                totalStudents += students;
                averageRating += courseRatingsDept
                    .Where(x => x.departmentID == p.departmentID).Sum(z => z.responses * z.ratings[0].averageRating);

                return new Department
                {
                    name = p.name,
                    collegeID = p.collegeID,
                    departmentID = p.departmentID,
                    collegeName = StaticData.collegeList.Where(u => u.collegeID == p.collegeID).FirstOrDefault().name,
                    rating = courseRatingsDept
                    .Where(x => x.departmentID == p.departmentID).Sum(z => ((double)z.responses / (double)responses) * z.ratings[0].averageRating).ToString("#.##"),
                    responses = responses.ToString(),
                    students = students.ToString(),
                    responseRate = ((double)responses / (double)students).ToString("p1")
                };
            }).Where(t => Convert.ToInt32(t.students) > 0).ToList();

            viewModel.departments = departments;
            viewModel.totalResponses = totalResponses.ToString("N0");
            viewModel.totalStudents = totalStudents.ToString("N0");
            viewModel.averageResponseRate = ((double)totalResponses / (double)totalStudents).ToString("p1");
            viewModel.averageRating = (averageRating / (double)totalResponses).ToString("#.##");
            viewModel.currentSemester = (GlobalVariables.CurrentSemester == "-1" ? "the past three semesters" : GlobalVariables.CurrentSemester.Split(' ')[1] + " " + GlobalVariables.CurrentSemester.Split(' ')[0]);

            return View(viewModel);
        }
    }
}