using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface IInstructorRepository<T>
{
    List<T> listAll();
    List<T> listByCollege(int collegeID);
    List<T> listByDepartment(int departmentID);
    List<T> listByCourse(int courseID);
    T getInstructorByID(int instructorID);
}
