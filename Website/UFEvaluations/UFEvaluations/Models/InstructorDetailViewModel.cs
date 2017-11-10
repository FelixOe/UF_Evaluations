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
        public List<CourseRatingDomain> courseRatingsAll { get; set; }
        public List<CourseDomain> courses { get; set; }
        public string firstTerm { get; set; }
        public string responsesAll { get; set; }
        public string studentsAll { get; set; }
        public string responseRateOverall { get; set; }
        public List<string> departments { get; set; }
        public string currentCategory { get; set; }
        public string totalResponses { get; set; }
        public string totalStudents { get; set; }
        public string averageResponseRate { get; set; }
        public string averageRating { get; set; }
        public string currentSemesterLow { get; set; }
        public string currentSemesterHigh { get; set; }
    }
}