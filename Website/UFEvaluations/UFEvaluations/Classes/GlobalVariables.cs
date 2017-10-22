using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public static class GlobalVariables
{
    public static string CurrentCategory
    {
        get { return (HttpContext.Current.Session["CurrentCategory"] != null ? HttpContext.Current.Session["CurrentCategory"].ToString() : "10"); }
        set { HttpContext.Current.Session["CurrentCategory"] = value; }
    }
    public static string CurrentSemester
    {
        get { return (HttpContext.Current.Session["CurrentSemester"] != null ? HttpContext.Current.Session["CurrentSemester"].ToString() : "-1"); }
        set { HttpContext.Current.Session["CurrentSemester"] = value; }
    }
    public static string CurrentSemesterLow
    {
        get { return (HttpContext.Current.Session["CurrentSemesterLow"] != null ? HttpContext.Current.Session["CurrentSemesterLow"].ToString() : StaticData.semesters.LastOrDefault().semester); }
        set { HttpContext.Current.Session["CurrentSemesterLow"] = value; }
    }
    public static string CurrentSemesterHigh
    {
        get { return (HttpContext.Current.Session["CurrentSemesterHigh"] != null ? HttpContext.Current.Session["CurrentSemesterHigh"].ToString() : StaticData.semesters.FirstOrDefault().semester); }
        set { HttpContext.Current.Session["CurrentSemesterHigh"] = value; }
    }
}