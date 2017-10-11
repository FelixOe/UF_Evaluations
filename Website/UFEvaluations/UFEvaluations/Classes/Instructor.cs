using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class Instructor
{
    public int instructorID { get; set; }
    public string lastName { get; set; }
    public string firstName { get; set; }
    public string department { get; set; }
    public string rating { get; set; }
    public string lastSemester { get; set; }

    public static Instructor getByID(int instructorID)
    {
        IInstructorRepository<Instructor> InstructorRepo = InstructorRepositorySQL.Instance;

        return InstructorRepo.getInstructorByID(instructorID);
    }
}