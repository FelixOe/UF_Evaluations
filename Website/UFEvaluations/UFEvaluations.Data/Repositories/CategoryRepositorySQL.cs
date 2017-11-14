using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace UFEvaluations.Data
{
    public class CategoryRepositorySQL : ICategoryRepository<Category>
    {
        private static CategoryRepositorySQL instance;
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionStringSQL"].ConnectionString;

        private CategoryRepositorySQL() { }

        public static CategoryRepositorySQL Instance
        {
            get
            {
                if (instance == null)
                    instance = new CategoryRepositorySQL();

                return instance;
            }
        }

        public List<Category> listAll()
        {
            List<Category> categories = new List<Category>();

            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "Select id, name FROM Categories;";
                cmd.Connection = conn;

                conn.Open();

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Category thisCategory = new Category();
                    thisCategory.categoryID = Convert.ToInt32(rdr[0]);
                    thisCategory.name = (string)rdr[1];

                    categories.Add(thisCategory);
                }

                conn.Close();
            }
            catch (Exception ex)
            {

            }

            return categories;
        }
    }
}