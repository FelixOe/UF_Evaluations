using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Models
{
    public class SearchSearchViewModel
    {
        public List<Pair<string, string>> searchResults { get; set; }
        public string searchTerm { get; set; }
    }
}