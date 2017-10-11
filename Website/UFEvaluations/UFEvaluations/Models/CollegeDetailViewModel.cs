using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Models
{
    public class CollegeDetailViewModel
    {
        public College college { get; set; }
        public List<Instructor> instructors { get; set; }
    }
}