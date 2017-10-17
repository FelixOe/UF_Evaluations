using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

public static class GlobalFunctions
{
    public static string escapeQuerystringElement(string QuerystringElement)
    {
        Regex reg = new Regex("[?&' ]"); //Pattern to find question marks, ampersands, single quotations, and spaces
        string filteredString = reg.Replace(QuerystringElement, "").ToLower(); //Filter all these characters and return lower-case string

        return filteredString;
    }
}