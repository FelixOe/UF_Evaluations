using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Models
{
    public class CourseListViewModel
    {
        public List<CourseDomain> courses { get; set; }
        public string totalResponses { get; set; }
        public string totalStudents { get; set; }
        public string totalInstructors { get; set; }
        public string averageResponseRate { get; set; }
        public string averageRating { get; set; }
        public string currentSemester { get; set; }
    }
}