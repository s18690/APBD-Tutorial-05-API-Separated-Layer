using Tutorial5.Models;

namespace Tutorial5.DTOs.Responses
{
    public class EnrollStudentResponse
    {
        public int Semester { get; set; }
        public Enrollment Enrollment { get; set; }

        public override string ToString()
        {
            return "Student has been enrolled to semester = " + Semester + ", \n" + Enrollment.ToString();
        }
    }
}