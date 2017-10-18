using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Course
{
    public int courseID { get; set; }
    public string title { get; set; }
    public string code { get; set; }
    public int departmentID { get; set; }
    public string departmentName { get; set; }

    public string rating { get; set; }
    public string responses { get; set; }
    public string students { get; set; }
    public string instructors { get; set; }
    public string responseRate { get; set; }

    public static Course getByID(int courseID)
    {
        ICourseRepository<Course> CourseRepo = CourseRepositorySQL.Instance;

        return CourseRepo.getCourseByID(courseID);
    }
}