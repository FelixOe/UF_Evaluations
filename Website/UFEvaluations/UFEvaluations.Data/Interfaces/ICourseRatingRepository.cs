using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFEvaluations.Data
{
    interface ICourseRatingRepository<T>
    {
        List<T> listAllByCategory(int categoryID);
        List<T> listByCategoryAndSemesters(int categoryID, params string[] semesters);
        List<T> listByInstructor(int instructorID);
        List<T> listByCourse(int courseID);
    }
}