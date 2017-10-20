using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UFEvaluations.Models;

namespace UFEvaluations.Controllers
{
    public class CollegesController : BaseController
    {
        public ViewResult Detail()
        {
            CollegeDetailViewModel viewModel = new CollegeDetailViewModel();

            if (Request.QueryString["college"] != null && StaticData.collegeList.Where(p => GlobalFunctions.escapeQuerystringElement(p.name) == GlobalFunctions.escapeQuerystringElement(Request.QueryString["college"].ToString())).Count() == 1)
            {
                College college = StaticData.collegeList
                    .Where(p => GlobalFunctions.escapeQuerystringElement(p.name) == GlobalFunctions.escapeQuerystringElement(Request.QueryString["college"].ToString()))
                    .FirstOrDefault();

                List<CourseRating> courseRatings = CourseRatingRepositorySQL.Instance.listByCategoryAndSemesters(
                    Convert.ToInt32(GlobalVariables.CurrentCategory),
                    (GlobalVariables.CurrentSemester == "-1" ? StaticData.semesters.Take(3).Select(y => y.semester).ToArray() : new[] { GlobalVariables.CurrentSemester }))
                    .Where(p => p.classSize >= p.responses)
                    .ToList();

                List<Instructor> instructors = InstructorRepositorySQL.Instance.listByCollege(college.collegeID)
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

                //Add department
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
                        responseRate = ((double)responses/(double)students).ToString("p1"),
                        //TODO: Retrieve all departments for each instructor
                        department = StaticData.departmentList.Where(x => x.departmentID == instructorDeptMapping.Where(a => a.instructorID == p.instructorID).FirstOrDefault().departmentID).FirstOrDefault().name,
                        lastSemester = (semesters.Count() > 0 ? semesters.FirstOrDefault() : ""),
                        rating = courseRatingInstructor.Sum(z => ((double)z.responses / (double)responses) * z.ratings[0].averageRating).ToString("#.##")
                    };
                }).ToList();

                viewModel.college = college;
                viewModel.instructors = instructors;
            }

            return View(viewModel);
        }

        public ViewResult List()
        {
            CollegeListViewModel viewModel = new CollegeListViewModel();

            List<CourseRating> courseRatings = CourseRatingRepositorySQL.Instance.listByCategoryAndSemesters(
                Convert.ToInt32(GlobalVariables.CurrentCategory),
                (GlobalVariables.CurrentSemester == "-1" ? StaticData.semesters.Take(3).Select(y => y.semester).ToArray() : new[] { GlobalVariables.CurrentSemester }))
                .Where(p => p.classSize >= p.responses)
                .ToList();

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
            }).OrderByDescending(s => s.rating).ToList();

            viewModel.colleges = topcollegesList.Select(p => StaticData.collegeList.Where(x => x.collegeID == p.collegeID).FirstOrDefault())
                .Select(x => new KeyValuePair<string, string>(x.name, topcollegesList.Where(y => y.collegeID == x.collegeID).FirstOrDefault().rating.ToString("#.##")))
                .ToList();

            return View(viewModel);
        }
    }
}