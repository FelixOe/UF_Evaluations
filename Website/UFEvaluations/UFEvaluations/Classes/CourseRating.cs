using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class CourseRating
{
    public int courseRatingID { get; set; }
    public int instructorID { get; set; }
    public int courseID { get; set; }
    public string section { get; set; }
    public string semester { get; set; }
    public int responses { get; set; }
    public int classSize { get; set; }
    public string term { get; set; }
    List<Rating> ratings { get; set; }
}