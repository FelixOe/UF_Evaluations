using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class SemesterComparer : IComparer<string>
{
    enum Term { Spring, Summer, Fall}

    public int Compare(string semester1, string semester2)
    {
        int year1, year2;
        Term term1, term2;

        if (semester1.Split(' ').Count() > 1 && semester2.Split(' ').Count() > 1)
        {
            year1 = Convert.ToInt32(semester1.Split(' ')[0]);
            year2 = Convert.ToInt32(semester2.Split(' ')[0]);

            term1 = (Term)Enum.Parse(typeof(Term), semester1.Split(' ')[1]);
            term2 = (Term)Enum.Parse(typeof(Term), semester2.Split(' ')[1]);

            if (year1 != year2)
                return year1.CompareTo(year2);
            else if (term1 != term2)
            {
                if (term1 < term2)
                    return -1;
                else if (term1 > term2)
                    return 1;
                else
                    return 0;
            }
            else
                return 0;
        }

        return 0;
    }
}