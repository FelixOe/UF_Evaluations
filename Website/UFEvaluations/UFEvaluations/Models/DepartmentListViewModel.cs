using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Models
{
    public class DepartmentListViewModel
    {
        public List<Department> departments { get; set; }
        public string totalResponses { get; set; }
        public string totalStudents { get; set; }
        public string averageResponseRate { get; set; }
        public string averageRating { get; set; }
        public string currentSemester { get; set; }
    }
}