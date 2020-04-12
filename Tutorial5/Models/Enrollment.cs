namespace Tutorial5.Models
{
    public class Enrollment
    {
        public int IdEnrollment { get; set; }
        public int Semester { get; set; }
        public int IdStudy { get; set; }
        public string StartDate { get; set; }

        public override string ToString()
        {
            return "Enrollment: " + "\n\t idEnrollment = " + IdEnrollment + ",\n\t semester = " + Semester + ",\n\t idStudy = " + IdStudy + ",\n\t startDate = " + StartDate;

        }
    }
}