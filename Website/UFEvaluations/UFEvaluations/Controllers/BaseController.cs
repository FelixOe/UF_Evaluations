using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UFEvaluations.Models;

namespace UFEvaluations.Controllers
{
    public class BaseController : Controller
    {
        public LayoutViewModel model;

        public BaseController()
        {
            model = new LayoutViewModel();

            model.searchScript = GlobalFunctions.createAutoCompleteScript();
            model.categories = StaticData.categoryList.Where(p => p.name != "NULL").ToList().Select(p => new Pair<Category, bool>(new Category {
                categoryID = p.categoryID,
                name = p.name
            }, false)).ToList();

            model.categories.Where(p => p.First.categoryID.ToString() == GlobalVariables.CurrentCategory).FirstOrDefault().Second = true;

            //Get semesters for entire site
            model.semesters = StaticData.semesters.Select(p => new Pair<Semester, bool>(new Semester
            {
                key = p.semester,
                semester = p.semester
            }, false)).ToList();
            model.semesters.Insert(0, new Pair<Semester, bool>(new Semester
            {
                key = "-1",
                semester = "Past 3 semesters"
            }, false));

            model.semesters.Where(p => p.First.key.ToString() == GlobalVariables.CurrentSemester).FirstOrDefault().Second = true;

            //Get low semesters for instructor detail
            model.semestersLow = StaticData.semesters.Select(p => new Pair<Semester, bool>(new Semester
            {
                key = p.semester,
                semester = p.semester
            }, false)).ToList();

            model.semestersLow.Where(p => p.First.key.ToString() == GlobalVariables.CurrentSemesterLow).FirstOrDefault().Second = true;

            //Get high semesters for instructor detail
            model.semestersHigh = StaticData.semesters.Select(p => new Pair<Semester, bool>(new Semester
            {
                key = p.semester,
                semester = p.semester
            }, false)).ToList();

            model.semestersHigh.Where(p => p.First.key.ToString() == GlobalVariables.CurrentSemesterHigh).FirstOrDefault().Second = true;

            ViewBag.LayoutViewModel = model;
        }
    }
}