using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Models
{
    public class HomeViewModel
    {
        public List<Course> topCourses { get; set; }
        public List<Instructor> topInstructors { get; set; }
        public List<Department> topDepartments { get; set; }
        public List<College> topColleges { get; set; }
    }
}