using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Models
{
    public class LayoutViewModel
    {
        public string searchScript { get; set; }
        public List<Pair<Category, bool>> categories { get; set; }
        public List<Pair<Semester, bool>> semesters { get; set; }
        public List<Pair<Semester, bool>> semestersLow { get; set; }
        public List<Pair<Semester, bool>> semestersHigh { get; set; }
    }
}