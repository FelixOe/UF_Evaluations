using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Models
{
    public class CollegeDetailViewModel
    {
        public College college { get; set; }
        public List<InstructorDomain> instructors { get; set; }
        public string totalResponses { get; set; }
        public string totalStudents { get; set; }
        public string averageResponseRate { get; set; }
        public string averageRating { get; set; }
        public string currentSemester { get; set; }
    }
}