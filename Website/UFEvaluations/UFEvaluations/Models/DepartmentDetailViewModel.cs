using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Models
{
    public class DepartmentDetailViewModel
    {
        public Department department { get; set; }
        public List<Instructor> instructors { get; set; }
    }
}