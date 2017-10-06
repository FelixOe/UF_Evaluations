using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class Rating
{
    public int ratingID { get; set; }
    public double averageRating { get; set; }
    public int courseRatingID { get; set; }
    public double standardDeviation { get; set; }
    public int categoryID { get; set; }
}