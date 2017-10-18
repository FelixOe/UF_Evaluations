using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Models
{
    public class CourseDetailViewModel
    {
        public Course course { get; set; }
        public List<Instructor> instructors { get; set; }
    }
}