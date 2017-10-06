using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class DepartmentRepositorySQL : IDepartmentRepository<Department>
{
    private static DepartmentRepositorySQL instance;
    private static readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionStringSQL"].ConnectionString;

    private DepartmentRepositorySQL() { }

    public static DepartmentRepositorySQL Instance
    {
        get
        {
            if (instance == null)
                instance = new DepartmentRepositorySQL();

            return instance;
        }
    }

    public List<Department> listAll()
    {
        List<Department> departments = new List<Department>();

        try
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "Select id, name, college_id FROM Deparments;";
            cmd.Connection = conn;

            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Department thisDepartment = new Department();
                thisDepartment.departmentID = Convert.ToInt32(rdr[0]);
                thisDepartment.name = (string)rdr[1];
                thisDepartment.collegeID = Convert.ToInt32(rdr[2]);

                departments.Add(thisDepartment);
            }

            conn.Close();
        }
        catch (Exception ex)
        {

        }

        return departments;
    }

    public List<Department> listByCollege(int collegeID)
    {
        if (collegeID < 0)
            throw new ArgumentOutOfRangeException();

        List<Department> departments = new List<Department>();

        try
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "Select id, name, college_id FROM Departments WHERE college_id = @college_id;";
            cmd.Parameters.AddWithValue("@college_id", collegeID);
            cmd.Connection = conn;

            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Department thisDepartment = new Department();
                thisDepartment.departmentID = Convert.ToInt32(rdr[0]);
                thisDepartment.name = (string)rdr[1];
                thisDepartment.collegeID = Convert.ToInt32(rdr[2]);

                departments.Add(thisDepartment);
            }

            conn.Close();
        }
        catch (Exception ex)
        {

        }

        return departments;
    }
}