using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class InstructorRepositorySQL : IInstructorRepository<Instructor>
{
    private static InstructorRepositorySQL instance;
    private static readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionStringSQL"].ConnectionString;

    private InstructorRepositorySQL() { }

    public static InstructorRepositorySQL Instance
    {
        get
        {
            if (instance == null)
                instance = new InstructorRepositorySQL();

            return instance;
        }
    }

    public List<Instructor> listAll()
    {
        List<Instructor> instructors = new List<Instructor>();

        try
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = @"Select id, full_name, last_name, first_name, [key] FROM Instructors;";
            cmd.Connection = conn;

            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Instructor thisInstructor = new Instructor();
                thisInstructor.instructorID = Convert.ToInt32(rdr[0]);
                thisInstructor.lastName = (string)rdr[1];
                thisInstructor.firstName = (string)rdr[2];

                instructors.Add(thisInstructor);
            }

            conn.Close();
        }
        catch (Exception ex)
        {

        }

        if (instructors.Count() == 0)
            return null;

        return instructors;
    }

    public List<Instructor> listByCollege(int collegeID)
    {
        if (collegeID < 0)
            throw new ArgumentOutOfRangeException();

        List<Instructor> instructors = new List<Instructor>();

        try
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = @" SELECT InstructorCollegeMapping.instructor_id, Instructors.last_name, Instructors.first_name FROM
                            (SELECT CourseRatings.instructor_id, CouColMap.college_id AS college_id FROM CourseRatings
                            INNER JOIN (SELECT Courses.id AS course_id, DeptColMap.college_id AS college_id FROM Courses INNER JOIN 
                            (SELECT Colleges.id AS college_id, Departments.id AS dept_id FROM Colleges INNER JOIN 
                            Departments ON Colleges.id = Departments.college_id GROUP BY Colleges.id, Departments.id) AS DeptColMap
                            ON Courses.department_id = DeptColMap.dept_id) AS CouColMap ON CourseRatings.course_id = CouColMap.course_id
                            GROUP BY CourseRatings.instructor_id, CouColMap.college_id) AS InstructorCollegeMapping
                            JOIN Instructors ON Instructors.id = InstructorCollegeMapping.instructor_id
                            WHERE college_id = @college_id;";
            cmd.Parameters.AddWithValue("@college_id", collegeID);
            cmd.Connection = conn;

            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Instructor thisInstructor = new Instructor();
                thisInstructor.instructorID = Convert.ToInt32(rdr[0]);
                thisInstructor.lastName = (string)rdr[1];
                thisInstructor.firstName = (string)rdr[2];

                instructors.Add(thisInstructor);
            }

            conn.Close();
        }
        catch (Exception ex)
        {

        }

        if (instructors.Count() == 0)
            return null;

        return instructors;
    }

    public List<Instructor> listByDepartment(int departmentID)
    {
        if (departmentID < 0)
            throw new ArgumentOutOfRangeException();

        List<Instructor> instructors = new List<Instructor>();

        try
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = @"SELECT Instructors.id, Instructors.last_name, Instructors.first_name FROM Instructors
                            INNER JOIN (SELECT CourseRatings.instructor_id, CourseDepartmentMapping.department_id FROM CourseRatings INNER JOIN
                            (SELECT Departments.id AS department_id, Courses.id AS course_id FROM Departments
                            INNER JOIN Courses ON Departments.id = Courses.department_id) AS CourseDepartmentMapping
                            ON CourseRatings.course_id = CourseDepartmentMapping.course_id 
                            GROUP BY CourseRatings.instructor_id, CourseDepartmentMapping.department_id) AS InstructorDeptMapping
                            ON Instructors.id = InstructorDeptMapping.instructor_id
                            WHERE InstructorDeptMapping.department_id = @department_id;";
            cmd.Parameters.AddWithValue("@department_id", departmentID);
            cmd.Connection = conn;

            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Instructor thisInstructor = new Instructor();
                thisInstructor.instructorID = Convert.ToInt32(rdr[0]);
                thisInstructor.lastName = (string)rdr[1];
                thisInstructor.firstName = (string)rdr[2];

                instructors.Add(thisInstructor);
            }

            conn.Close();
        }
        catch (Exception ex)
        {

        }

        if (instructors.Count() == 0)
            return null;

        return instructors;
    }

    public List<Instructor> listByCourse(int courseID)
    {
        if (courseID < 0)
            throw new ArgumentOutOfRangeException();

        List<Instructor> instructors = new List<Instructor>();

        try
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = @"SELECT Instructors.id, Instructors.last_name, Instructors.first_name FROM Instructors
                            INNER JOIN (SELECT CourseRatings.course_id, CourseRatings.instructor_id FROM CourseRatings
                            GROUP BY CourseRatings.course_id, CourseRatings.instructor_id) AS CourseInstructorsMapping
                            ON Instructors.id = CourseInstructorsMapping.instructor_id
                            WHERE course_id = @course_id;";
            cmd.Parameters.AddWithValue("@course_id", courseID);
            cmd.Connection = conn;

            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Instructor thisInstructor = new Instructor();
                thisInstructor.instructorID = Convert.ToInt32(rdr[0]);
                thisInstructor.lastName = (string)rdr[1];
                thisInstructor.firstName = (string)rdr[2];

                instructors.Add(thisInstructor);
            }

            conn.Close();
        }
        catch (Exception ex)
        {

        }

        if (instructors.Count() == 0)
            return null;

        return instructors;
    }

    public Instructor getInstructorByID(int instructorID)
    {
        if (instructorID < 0)
            throw new ArgumentOutOfRangeException();

        List<Instructor> instructors = new List<Instructor>();

        try
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "Select id, full_name, last_name, first_name, [key] FROM Instructors WHERE id = @instructor_id;";
            cmd.Parameters.AddWithValue("@instructor_id", instructorID);
            cmd.Connection = conn;

            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Instructor thisInstructor = new Instructor();
                thisInstructor.instructorID = Convert.ToInt32(rdr[0]);
                thisInstructor.lastName = (string)rdr[1];
                thisInstructor.firstName = (string)rdr[2];

                instructors.Add(thisInstructor);
            }

            conn.Close();
        }
        catch (Exception ex)
        {

        }

        if (instructors.Count() == 0)
            return null;

        return instructors.FirstOrDefault();
    }
}