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

                List<Instructor> instructorsAll = InstructorRepositorySQL.Instance.listByCollege(college.collegeID)
                    .Where(p => courseRatings.Select(u => u.instructorID).Distinct().Contains(p.instructorID))
                    .ToList();

                var instructorDeptMapping = instructorsAll.Select(p => {
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

                //Add department
                List<InstructorDomain> instructors = instructorsAll.Select(p => {
                    var courseRatingInstructor = courseRatings.Where(x => x.instructorID == p.instructorID && StaticData.departmentList
                    .Where(y => y.departmentID == StaticData.courseDeptMapping[x.courseID.ToString()]).FirstOrDefault().collegeID == college.collegeID);
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
                        responseRate = ((double)responses/(double)students).ToString("p1"),
                        department = StaticData.departmentList.Where(x => x.departmentID == instructorDeptMapping.Where(a => a.instructorID == p.instructorID).FirstOrDefault().departmentID).FirstOrDefault().name,
                        lastSemester = (semesters.Count() > 0 ? semesters.FirstOrDefault() : ""),
                        rating = courseRatingInstructor.Sum(z => ((double)z.responses / (double)responses) * z.ratings[0].averageRating).ToString("#.##")
                    };
                }).ToList();

                viewModel.college = college;
                viewModel.instructors = instructors;
                viewModel.totalResponses = totalResponses.ToString("N0");
                viewModel.totalStudents = totalStudents.ToString("N0");
                viewModel.averageResponseRate = ((double)totalResponses / (double)totalStudents).ToString("p1");
                viewModel.averageRating = (averageRating / (double)totalResponses).ToString("#.##");
                viewModel.currentSemester = (GlobalVariables.CurrentSemester == "-1" ? "the past three semesters" : GlobalVariables.CurrentSemester.Split(' ')[1] + " " + GlobalVariables.CurrentSemester.Split(' ')[0]);
            }
            else
                throw new HttpException(404, "College not found!");

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
            int totalResponses = 0;
            int totalStudents = 0;
            double averageRating = 0.0;

            List<CollegeDomain> colleges = StaticData.collegeList.Select(p =>
            {
                var responses = courseRatingsCol.Where(x => x.collegeID == p.collegeID).Select(y => y.responses).Sum(z => z);
                var students = courseRatingsCol.Where(x => x.collegeID == p.collegeID).Select(y => y.classSize).Sum(z => z);

                totalResponses += responses;
                totalStudents += students;
                averageRating += courseRatingsCol
                        .Where(x => x.collegeID == p.collegeID).Sum(z => z.responses * z.ratings[0].averageRating);

                return new CollegeDomain
                {
                    collegeID = p.collegeID,
                    name = p.name,
                    rating = courseRatingsCol
                    .Where(x => x.collegeID == p.collegeID).Sum(z => ((double)z.responses / (double)responses) * z.ratings[0].averageRating).ToString("#.##"),
                    responses = responses.ToString(),
                    students = students.ToString(),
                    responseRate = ((double)responses / (double)students).ToString("p1")
                };
            }).OrderByDescending(s => s.rating).ToList();

            viewModel.colleges = colleges;
            viewModel.totalResponses = totalResponses.ToString("N0");
            viewModel.totalStudents = totalStudents.ToString("N0");
            viewModel.averageResponseRate = ((double)totalResponses / (double)totalStudents).ToString("p1");
            viewModel.averageRating = (averageRating / (double)totalResponses).ToString("#.##");
            viewModel.currentSemester = (GlobalVariables.CurrentSemester == "-1" ? "the past three semesters" : GlobalVariables.CurrentSemester.Split(' ')[1] + " " + GlobalVariables.CurrentSemester.Split(' ')[0]);

            return View(viewModel);
        }
    }
}