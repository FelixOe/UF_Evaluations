using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFEvaluations.Data
{
    interface IDepartmentRepository<T>
    {
        List<T> listAll();
        List<T> listByCollege(int collegeID);
    }
}