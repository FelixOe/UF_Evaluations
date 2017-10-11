using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UFEvaluations.Models;

namespace UFEvaluations.Controllers
{
    public class CollegesController : Controller
    {
        public ViewResult Detail()
        {
            CollegeDetailViewModel viewModel = new CollegeDetailViewModel();

            if (Request.QueryString["college"] != null && StaticData.collegeList.Where(p => p.name.ToLower() == Request.QueryString["college"].ToString().ToLower()).Count() == 1)
            {
                College college = StaticData.collegeList.Where(p => p.name.ToLower() == Request.QueryString["college"].ToString().ToLower()).FirstOrDefault();
                string currentYear = DateTime.Now.Year.ToString();

                List<CourseRating> courseRatingsCurrentYear = StaticData.overallRatingsList.Where(p => p.semester.Contains(currentYear) && p.classSize >= p.responses).ToList();

                List<Instructor> instructors = InstructorRepositorySQL.Instance.listByCollege(college.collegeID).Where(p => courseRatingsCurrentYear.Select(u => u.instructorID).Distinct().Contains(p.instructorID)).ToList();

                var instructorDeptMapping = instructors.Select(p => {
                    CourseRating firstRating = courseRatingsCurrentYear.Where(u => u.instructorID == p.instructorID).FirstOrDefault();
                    var dept = StaticData.courseList.Where(t => t.courseID == firstRating.courseID).FirstOrDefault().departmentID;
                    return new
                    {
                        instructorID = p.instructorID,
                        departmentID = dept
                    };
                }).ToList();

                //Add department
                instructors = instructors.Select(p => {
                    var courseRatingInstructor = courseRatingsCurrentYear.Where(x => x.instructorID == p.instructorID);
                    var responses = courseRatingInstructor.Select(y => y.responses).Sum(z => z);
                    var semesters = courseRatingInstructor.Select(v =>
                    {
                        //TODO: Get correct order b/w Fall, Spring, and Summer
                        //TODO: Get the past 3 semesters, not just current year
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
            string currentYear = DateTime.Now.Year.ToString();
            List<CourseRating> courseRatingsCurrentYear = StaticData.overallRatingsList.Where(p => p.semester.Contains(currentYear) && p.classSize >= p.responses).ToList();

            var courseRatingsCurrentYearDept = courseRatingsCurrentYear.Join(StaticData.courseList, prim => prim.courseID, fore => fore.courseID,
(prim, fore) => new { fore.departmentID, prim.classSize, prim.responses, prim.ratings });

            var departmentList = StaticData.departmentList.Select(t => t.departmentID).Select(p =>
            {
                var responses = courseRatingsCurrentYearDept.Where(x => x.departmentID == p).Select(y => y.responses).Sum(z => z);
                return new
                {
                    departmentID = p,
                    rating = courseRatingsCurrentYearDept
                    .Where(x => x.departmentID == p).Sum(z => ((double)z.responses / (double)responses) * z.ratings[0].averageRating),
                    responses = responses,
                    classSizes = courseRatingsCurrentYearDept.Where(x => x.departmentID == p).Select(y => y.classSize).Sum(z => z),
                    collegeID = StaticData.departmentList.Where(a => a.departmentID == p).FirstOrDefault().collegeID
                };
            }).ToList();

            var courseRatingsCurrentYearCol = courseRatingsCurrentYearDept.Join(StaticData.departmentList, prim => prim.departmentID, fore => fore.departmentID,
(prim, fore) => new { fore.collegeID, prim.classSize, prim.responses, prim.ratings });

            var topcollegesList = StaticData.collegeList.Select(t => t.collegeID).Select(p =>
            {
                var responses = courseRatingsCurrentYearCol.Where(x => x.collegeID == p).Select(y => y.responses).Sum(z => z);
                return new
                {
                    collegeID = p,
                    rating = courseRatingsCurrentYearCol
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