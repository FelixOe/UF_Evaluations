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
}