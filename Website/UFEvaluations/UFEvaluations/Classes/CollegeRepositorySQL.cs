using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

public class CollegeRepositorySQL : ICollegeRepository<College>
{
    private static CollegeRepositorySQL instance;
    private static readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionStringSQL"].ConnectionString;

    private CollegeRepositorySQL() { }

    public static CollegeRepositorySQL Instance
    {
        get
        {
            if (instance == null)
                instance = new CollegeRepositorySQL();

            return instance;
        }
    }

    public List<College> listAll()
    {
        List<College> colleges = new List<College>();

        try
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "Select id, name FROM Colleges;";
            cmd.Connection = conn;

            conn.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                College thisCollege = new College();
                thisCollege.collegeID = Convert.ToInt32(rdr[0]);
                thisCollege.name = (string)rdr[1];

                colleges.Add(thisCollege);
            }

            conn.Close();
        }
        catch (Exception ex)
        {

        }

        return colleges;
    }
}