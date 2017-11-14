using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace UFEvaluations.Data
{
    public class CourseRepositorySQL : ICourseRepository<Course>
    {
        private static CourseRepositorySQL instance;
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionStringSQL"].ConnectionString;

        private CourseRepositorySQL() { }

        public static CourseRepositorySQL Instance
        {
            get
            {
                if (instance == null)
                    instance = new CourseRepositorySQL();

                return instance;
            }
        }

        public List<Course> listAll()
        {
            List<Course> courses = new List<Course>();

            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "Select id, name, code, department_id FROM Courses;";
                cmd.Connection = conn;

                conn.Open();

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Course thisCourse = new Course();
                    thisCourse.courseID = Convert.ToInt32(rdr[0]);
                    thisCourse.title = (string)rdr[1];
                    thisCourse.code = (string)rdr[2];
                    thisCourse.departmentID = Convert.ToInt32(rdr[3]);

                    courses.Add(thisCourse);
                }

                conn.Close();
            }
            catch (Exception ex)
            {

            }

            return courses;
        }

        public Course getCourseByID(int courseID)
        {
            if (courseID < 0)
                throw new ArgumentOutOfRangeException();

            List<Course> courses = new List<Course>();

            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "Select id, name, code, department_id FROM Courses WHERE id = @course_id;";
                cmd.Parameters.AddWithValue("@course_id", courseID);
                cmd.Connection = conn;

                conn.Open();

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Course thisCourse = new Course();
                    thisCourse.courseID = Convert.ToInt32(rdr[0]);
                    thisCourse.title = (string)rdr[1];
                    thisCourse.code = (string)rdr[2];
                    thisCourse.departmentID = Convert.ToInt32(rdr[3]);

                    courses.Add(thisCourse);
                }

                conn.Close();
            }
            catch (Exception ex)
            {

            }

            if (courses.Count() == 0)
                return null;

            return courses.FirstOrDefault();
        }
    }
}