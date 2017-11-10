using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Models
{
    public class CourseDetailViewModel
    {
        public Course course { get; set; }
        public List<InstructorDomain> instructors { get; set; }
        public string totalResponses { get; set; }
        public string totalStudents { get; set; }
        public string averageResponseRate { get; set; }
        public string averageRating { get; set; }
        public string currentSemesterLow { get; set; }
        public string currentSemesterHigh { get; set; }
    }
}