using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UFEvaluations.Data
{
    public class CategoryRepositoryXML : ICategoryRepository<Category>
    {
        private static CategoryRepositoryXML instance;

        private CategoryRepositoryXML() { }

        public static CategoryRepositoryXML Instance
        {
            get
            {
                if (instance == null)
                    instance = new CategoryRepositoryXML();

                return instance;
            }
        }

        public List<Category> listAll()
        {
            List<Category> categories = new List<Category>();

            return categories;
        }
    }
}