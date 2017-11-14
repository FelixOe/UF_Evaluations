using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFEvaluations.Data
{
    interface ICourseRepository<T>
    {
        List<T> listAll();
        T getCourseByID(int courseID);
    }
}