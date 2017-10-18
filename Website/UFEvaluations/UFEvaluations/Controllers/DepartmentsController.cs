using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UFEvaluations.Models;

namespace UFEvaluations.Controllers
{
    public class DepartmentsController : Controller
    {
        public ViewResult Detail()
        {
            DepartmentDetailViewModel viewModel = new DepartmentDetailViewModel();

            if (Request.QueryString["department"] != null && StaticData.departmentList.Where(p => GlobalFunctions.escapeQuerystringElement(p.name) == GlobalFunctions.escapeQuerystringElement(Request.QueryString["department"].ToString())).Count() == 1)
            {
                Department department = StaticData.departmentList
                    .Where(p => GlobalFunctions.escapeQuerystringElement(p.name) == GlobalFunctions.escapeQuerystringElement(Request.QueryString["department"].ToString()))
                    .FirstOrDefault();

                List<CourseRating> courseRatings = StaticData.overallRatingsList
                    .Where(p => StaticData.termsToDisplay.Contains(p.semester) && p.classSize >= p.responses)
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

                instructors = instructors.Select(p => {
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

                viewModel.department = department;
                viewModel.instructors = instructors;
            }

            return View(viewModel);
        }

        public ActionResult List()
        {
            DepartmentListViewModel viewModel = new DepartmentListViewModel();

            List<CourseRating> courseRatings = StaticData.overallRatingsList
                .Where(p => StaticData.termsToDisplay.Contains(p.semester) && p.classSize >= p.responses)
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

            List<Department> departments = StaticData.departmentList.Select(p =>
            {
                var responses = courseRatingsDept.Where(x => x.departmentID == p.departmentID).Select(y => y.responses).Sum(z => z);
                var students = courseRatingsDept.Where(x => x.departmentID == p.departmentID).Select(y => y.classSize).Sum(z => z);
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

            return View(viewModel);
        }
    }
}