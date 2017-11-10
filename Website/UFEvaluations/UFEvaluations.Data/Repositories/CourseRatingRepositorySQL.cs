using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class CourseRatingRepositorySQL : ICourseRatingRepository<CourseRating>
{
    private static CourseRatingRepositorySQL instance;
    private static readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionStringSQL"].ConnectionString;

    private CourseRatingRepositorySQL() { }

    public static CourseRatingRepositorySQL Instance
    {
        get
        {
            if (instance == null)
                instance = new CourseRatingRepositorySQL();

            return instance;
        }
    }

    public List<CourseRating> listAllByCategory(int categoryID)
    {
        List<CourseRating> courseRatings = new List<CourseRating>();

        try
        {
            SqlConnection conn = new SqlConnection(connectionString);

            string query = @"Select CourseRatings.id AS course_rating_id, CourseRatings.course_id, CourseRatings.instructor_id, CourseRatings.section, CourseRatings.semester, CourseRatings.responses,
                            CourseRatings.class_size, CourseRatings.term, Scores.id, Scores.score, Scores.st_dev, Scores.category_id FROM CourseRatings
                            JOIN Scores ON CourseRatings.id = Scores.course_rating_id
                            WHERE Scores.category_id = @category_id;";

            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            adapter.SelectCommand.Parameters.AddWithValue("@category_id", categoryID);

            DataTable courseRatingsTable = new DataTable();
            adapter.Fill(courseRatingsTable);

            courseRatings = courseRatingsTable.AsEnumerable().Select(p => new CourseRating {
                courseRatingID = Convert.ToInt32(p.ItemArray[0].ToString()),
                courseID = Convert.ToInt32(p.ItemArray[1].ToString()),
                instructorID = Convert.ToInt32(p.ItemArray[2].ToString()),
                section = p.ItemArray[3].ToString(),
                semester = p.ItemArray[4].ToString(),
                responses = Convert.ToInt32(p.ItemArray[5].ToString()),
                classSize = Convert.ToInt32(p.ItemArray[6].ToString()),
                term = p.ItemArray[7].ToString(),
                ratings = new List<Rating>() { new Rating {
                    courseRatingID = Convert.ToInt32(p.ItemArray[0].ToString()),
                    ratingID = Convert.ToInt32(p.ItemArray[8].ToString()),
                    averageRating = Convert.ToDouble(p.ItemArray[9].ToString()),
                    standardDeviation = Convert.ToDouble(p.ItemArray[10].ToString()),
                    categoryID = Convert.ToInt32(p.ItemArray[11].ToString())
                } }
            }).ToList();
        }
        catch (Exception ex)
        {

        }

        return courseRatings;
    }

    public List<CourseRating> listByCategoryAndSemesters(int categoryID, params string[] semesters)
    {
        List<CourseRating> courseRatings = new List<CourseRating>();

        if (semesters.Count() <= 0)
            throw new ArgumentOutOfRangeException();

        try
        {
            SqlConnection conn = new SqlConnection(connectionString);

            string query = @"Select CourseRatings.id AS course_rating_id, CourseRatings.course_id, CourseRatings.instructor_id, CourseRatings.section, CourseRatings.semester, CourseRatings.responses,
                            CourseRatings.class_size, CourseRatings.term, Scores.id, Scores.score, Scores.st_dev, Scores.category_id FROM CourseRatings
                            JOIN Scores ON CourseRatings.id = Scores.course_rating_id
                            WHERE Scores.category_id = @category_id AND (";

            for (int i = 0; i < semesters.Count(); i++)
                query += (i == 0 ? "" : " OR ") + "CourseRatings.semester = @semester" + i;

            query += ");";

            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            adapter.SelectCommand.Parameters.AddWithValue("@category_id", categoryID);

            for(int i = 0; i < semesters.Count(); i++)
                adapter.SelectCommand.Parameters.AddWithValue("@semester" + i, semesters[i]);

            DataTable courseRatingsTable = new DataTable();
            adapter.Fill(courseRatingsTable);

            courseRatings = courseRatingsTable.AsEnumerable().Select(p => new CourseRating
            {
                courseRatingID = Convert.ToInt32(p.ItemArray[0].ToString()),
                courseID = Convert.ToInt32(p.ItemArray[1].ToString()),
                instructorID = Convert.ToInt32(p.ItemArray[2].ToString()),
                section = p.ItemArray[3].ToString(),
                semester = p.ItemArray[4].ToString(),
                responses = Convert.ToInt32(p.ItemArray[5].ToString()),
                classSize = Convert.ToInt32(p.ItemArray[6].ToString()),
                term = p.ItemArray[7].ToString(),
                ratings = new List<Rating>() { new Rating {
                    courseRatingID = Convert.ToInt32(p.ItemArray[0].ToString()),
                    ratingID = Convert.ToInt32(p.ItemArray[8].ToString()),
                    averageRating = Convert.ToDouble(p.ItemArray[9].ToString()),
                    standardDeviation = Convert.ToDouble(p.ItemArray[10].ToString()),
                    categoryID = Convert.ToInt32(p.ItemArray[11].ToString())
                } }
            }).ToList();
        }
        catch (Exception ex)
        {

        }

        return courseRatings;
    }

    public List<CourseRating> listByInstructor(int instructorID)
    {
        List<CourseRating> courseRatings = new List<CourseRating>();

        try
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = @"Select CourseRatings.id AS course_rating_id, CourseRatings.course_id, CourseRatings.instructor_id, CourseRatings.section, CourseRatings.semester, CourseRatings.responses,
                            CourseRatings.class_size, CourseRatings.term, Scores.id, Scores.score, Scores.st_dev, Scores.category_id FROM CourseRatings
                            JOIN Scores ON CourseRatings.id = Scores.course_rating_id
                            WHERE CourseRatings.instructor_id = @instructor_id;";
            cmd.Parameters.AddWithValue("@instructor_id", instructorID);
            cmd.Connection = conn;

            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                if (!courseRatings.Select(p => p.courseRatingID).Contains(Convert.ToInt32(rdr[0])))
                {
                    CourseRating thisCourseRating = new CourseRating();
                    thisCourseRating.courseRatingID = Convert.ToInt32(rdr[0]);
                    thisCourseRating.courseID = Convert.ToInt32(rdr[1]);
                    thisCourseRating.instructorID = Convert.ToInt32(rdr[2]);
                    thisCourseRating.section = (string)rdr[3];
                    thisCourseRating.semester = (string)rdr[4];
                    thisCourseRating.responses = Convert.ToInt32(rdr[5]);
                    thisCourseRating.classSize = Convert.ToInt32(rdr[6]);
                    thisCourseRating.term = (string)rdr[7];
                    thisCourseRating.ratings = new List<Rating>();

                    courseRatings.Add(thisCourseRating);
                }

                Rating thisRating = new Rating();
                thisRating.courseRatingID = Convert.ToInt32(rdr[0]);
                thisRating.ratingID = Convert.ToInt32(rdr[8]);
                thisRating.averageRating = Convert.ToDouble(rdr[9]);
                thisRating.standardDeviation = Convert.ToDouble(rdr[10]);
                thisRating.categoryID = Convert.ToInt32(rdr[11]);

                courseRatings.Where(p => p.courseRatingID == thisRating.courseRatingID).FirstOrDefault().ratings.Add(thisRating);
            }

            conn.Close();
        }
        catch (Exception ex)
        {

        }

        return courseRatings;
    }
    public List<CourseRating> listByCourse(int courseID)
    {
        List<CourseRating> courseRatings = new List<CourseRating>();

        try
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = @"Select CourseRatings.id AS course_rating_id, CourseRatings.course_id, CourseRatings.instructor_id, CourseRatings.section, CourseRatings.semester, CourseRatings.responses,
                            CourseRatings.class_size, CourseRatings.term, Scores.id, Scores.score, Scores.st_dev, Scores.category_id FROM CourseRatings
                            JOIN Scores ON CourseRatings.id = Scores.course_rating_id
                            WHERE CourseRatings.course_id = @course_id;";
            cmd.Parameters.AddWithValue("@course_id", courseID);
            cmd.Connection = conn;

            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                if (!courseRatings.Select(p => p.courseRatingID).Contains(Convert.ToInt32(rdr[0])))
                {
                    CourseRating thisCourseRating = new CourseRating();
                    thisCourseRating.courseRatingID = Convert.ToInt32(rdr[0]);
                    thisCourseRating.courseID = Convert.ToInt32(rdr[1]);
                    thisCourseRating.instructorID = Convert.ToInt32(rdr[2]);
                    thisCourseRating.section = (string)rdr[3];
                    thisCourseRating.semester = (string)rdr[4];
                    thisCourseRating.responses = Convert.ToInt32(rdr[5]);
                    thisCourseRating.classSize = Convert.ToInt32(rdr[6]);
                    thisCourseRating.term = (string)rdr[7];
                    thisCourseRating.ratings = new List<Rating>();

                    courseRatings.Add(thisCourseRating);
                }

                Rating thisRating = new Rating();
                thisRating.courseRatingID = Convert.ToInt32(rdr[0]);
                thisRating.ratingID = Convert.ToInt32(rdr[8]);
                thisRating.averageRating = Convert.ToDouble(rdr[9]);
                thisRating.standardDeviation = Convert.ToDouble(rdr[10]);
                thisRating.categoryID = Convert.ToInt32(rdr[11]);

                courseRatings.Where(p => p.courseRatingID == thisRating.courseRatingID).FirstOrDefault().ratings.Add(thisRating);
            }

            conn.Close();
        }
        catch (Exception ex)
        {

        }

        return courseRatings;
    }
}