﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace UFEvaluations
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Load static data
            StaticData.categoryList = CategoryRepositorySQL.Instance.listAll();
            StaticData.courseList = CourseRepositorySQL.Instance.listAll();
            StaticData.courseDeptMapping = new Dictionary<string, int>();
            foreach (Course c in StaticData.courseList)
                StaticData.courseDeptMapping.Add(c.courseID.ToString(), c.departmentID);
            StaticData.instructorList = InstructorRepositorySQL.Instance.listAll();
            StaticData.departmentList = DepartmentRepositorySQL.Instance.listAll();
            StaticData.collegeList = CollegeRepositorySQL.Instance.listAll();
            StaticData.collegeList = StaticData.collegeList.Select(p => new College
            {
                collegeID = p.collegeID,
                name = (p.name.ToLower().Contains("college") ? System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(p.name.ToLower()).Replace("Of", "of") : "College of " + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(p.name.ToLower()))
            }).ToList();
            StaticData.overallRatingsList = CourseRatingRepositorySQL.Instance.listAllByCategory(10);
            StaticData.searchTerms = new List<string>();
            StaticData.searchTerms = StaticData.searchTerms.Union(StaticData.instructorList.Select(p => p.lastName.Trim() + " " + p.firstName.Trim())).ToList();
            StaticData.searchTerms = StaticData.searchTerms.Union(StaticData.collegeList.Select(p => p.name)).ToList();
            StaticData.searchTerms = StaticData.searchTerms.Union(StaticData.departmentList.Select(p => p.name)).ToList();
            StaticData.searchTerms = StaticData.searchTerms.Union(StaticData.courseList.Select(p => p.code + " - " + p.title)).ToList();
            StaticData.searchTerms.OrderBy(p => p);

            StaticData.termsToDisplay = new List<string>();
            StaticData.termsToDisplay.Add("2017 Summer");
            StaticData.termsToDisplay.Add("2017 Spring");
            StaticData.termsToDisplay.Add("2016 Fall");

            var terms = StaticData.overallRatingsList.Select(p => p.term).Distinct();

            StaticData.semesters = terms.Select(p => new Semester { key = p, semester = StaticData.overallRatingsList.Where(y => y.term == p).FirstOrDefault().semester }).Distinct()
                .OrderByDescending(p => p.semester, new SemesterComparer()).ToList();
        }
    }
}
