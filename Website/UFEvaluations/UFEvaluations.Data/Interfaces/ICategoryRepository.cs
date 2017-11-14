using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFEvaluations.Data
{
    interface ICategoryRepository<T>
    {
        List<T> listAll();
    }
}