using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Models
{
    public class HomeViewModel
    {
        public List<KeyValuePair<string, string>> topCourses { get; set; }
        public List<KeyValuePair<string, string>> topInstructors { get; set; }
        public List<KeyValuePair<string, string>> topDepartments { get; set; }
        public List<KeyValuePair<string, string>> topColleges { get; set; }
    }
}