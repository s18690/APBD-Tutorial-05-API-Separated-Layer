using System.Collections.Generic;
using Tutorial5.DTOs.Requests;
using Tutorial5.DTOs.Responses;
using Tutorial5.Models;

namespace Tutorial5.Services
{
    public interface IDbService
    {
        IEnumerable<Student> GetStudents();

        Enrollment GetEnrollOfStudByIndNo(string indexNumber);

        Student GetStudentByIndex(string idexNumber);



        EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);

        PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request);



        void SaveLogData(IEnumerable<string> data);
    }
}