using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Models
{
    public class InstructorDetailViewModel
    {
        public Instructor instructor { get; set; }
        public List<OverallRating> overallRatings { get; set; }
        public List<CourseRating> courseRatingsAll { get; set; }
        public List<Course> courses { get; set; }
        public string firstTerm { get; set; }
        public string responsesAll { get; set; }
        public string studentsAll { get; set; }
        public string responseRateOverall { get; set; }
        public List<string> departments { get; set; }
        public string currentCategory { get; set; }
    }
}