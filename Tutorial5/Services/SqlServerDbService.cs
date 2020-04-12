using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Tutorial5.DTOs.Requests;
using Tutorial5.DTOs.Responses;
using Tutorial5.Models;

namespace Tutorial5.Services
{
    public class SqlServerDbService : IDbService
    {
        private string ConnString = "Data Source=db-mssql16.pjwstk.edu.pl;Initial Catalog=s18690;User ID=apbds18690;Password=admin";

        #region StudentsController

        public IEnumerable<Student> GetStudents()
        {
            var listOfStudents = new List<Student>();

            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT * FROM Student";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString(); 
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.IdEnrollment = Convert.ToInt32(dr["IdEnrollment"]);
                    listOfStudents.Add(st);
                }
            }
            return listOfStudents;
        }

        public Enrollment GetEnrollOfStudByIndNo(string indexNumber)
        {
            var enr = new Enrollment();
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT e.IdEnrollment, e.semester, e.idstudy, e.startdate FROM Enrollment e, Student st WHERE st.IndexNumber = @index AND st.IdEnrollment = e.IdEnrollment";
                com.Parameters.AddWithValue("index", indexNumber);


                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (!dr.Read())
                {
                    throw new ArgumentNullException("Enrollment of student with given index number hasn't been found");
                }
                enr.IdEnrollment = Convert.ToInt32(dr["IdEnrollment"]);
                enr.Semester = Convert.ToInt32(dr["Semester"]);
                enr.IdStudy = Convert.ToInt32(dr["IdStudy"]);
                enr.StartDate = dr["StartDate"].ToString();
            }
            return enr;
        }

        public Student GetStudentByIndex(string indexNumber)
        {
            using (SqlConnection con = new SqlConnection(ConnString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "SELECT * FROM Student WHERE IndexNumber = @index";
                com.Parameters.AddWithValue("index", indexNumber);
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student();
                    st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.IdEnrollment = Convert.ToInt32(dr["IdEnrollment"]);
                    return st;
                }
            }
            return null;
        }

        #endregion

        #region EnrollmentsController

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            EnrollStudentResponse response = null;
            Enrollment respEnrollment = new Enrollment();
            SqlTransaction tran = null;

            using (var con = new SqlConnection(ConnString))
            using (var com = new SqlCommand())
            {
                #region 2.

                com.CommandText = "SELECT * FROM Studies WHERE Name = @StudyName";
                com.Parameters.AddWithValue("StudyName", request.Studies);

                com.Connection = con;
                con.Open();
                tran = con.BeginTransaction();

                com.Transaction = tran;
                SqlDataReader dr = com.ExecuteReader();
                if (!dr.Read()) 
                {

                    dr.Close();
                    tran.Rollback();
                    dr.Dispose();
                    throw new ArgumentException("Studies not found");
                }
                int idStudy = (int)dr["IdStudy"];
                dr.Close();
                #endregion

                #region 3.
                int idEnrollment = 0;

                com.CommandText = "SELECT * FROM Enrollment WHERE Semester = 1 AND IdStudy = @IdStudy";
                com.Parameters.AddWithValue("IdStudy", idStudy);
                com.Transaction = tran;
                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    idEnrollment = (int)dr["IdEnrollment"];
                }
                else
                {
                    dr.Close();
                    com.CommandText = "SELECT IdEnrollment FROM Enrollment";
                    com.Transaction = tran;
                    dr = com.ExecuteReader();
                    int newIdEnrollment = 0;
                    while (dr.Read())
                    {
                        int maxIdEnrollment = (int)dr["IdEnrollment"];
                        if (maxIdEnrollment > newIdEnrollment) newIdEnrollment = maxIdEnrollment;
                    }
                    newIdEnrollment++; 
                    dr.Close();


                    com.CommandText = "INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate) VALUES (@NewIdEnrollment, 1, @IdStudy, convert(varchar, getdate(), 110))";

                    com.Parameters.AddWithValue("NewIdEnrollment", newIdEnrollment);
                    com.Transaction = tran;
                    com.ExecuteNonQuery();
                    idEnrollment = newIdEnrollment;
                }
                dr.Close();
                #endregion

                #region 4.

                com.CommandText = "SELECT * FROM Student WHERE IndexNumber = @IndexNumber";
                com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                com.Transaction = tran;
                dr = com.ExecuteReader();
                if (dr.Read()) 
                {
                    dr.Close();
                    tran.Rollback();
                    dr.Dispose();
                    throw new InvalidOperationException("Student with given index number already exists");
                }
                dr.Close();

                #region additional - not needed since we have new IndexNumber from request

                */
                #endregion

                com.CommandText = "INSERT INTO Student (IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@IndexNumber, @Firstname, @LastName, convert(datetime, @BirthDate, 104), @IdEnrollment)";
                com.Parameters.AddWithValue("FirstName", request.FirstName);
                com.Parameters.AddWithValue("LastName", request.LastName);
                com.Parameters.AddWithValue("BirthDate", request.BirthDate);
                com.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                com.Transaction = tran;
                com.ExecuteNonQuery();
                dr.Close();
                #endregion

                #region 5. RESPONSE
 
                com.CommandText = "SELECT * FROM Enrollment WHERE IdEnrollment = @IdEnrollment";
                com.Transaction = tran;
                dr = com.ExecuteReader();
                dr.Read(); 

                response = new EnrollStudentResponse();
                response.Semester = 1; 
                response.Enrollment = respEnrollment;
                respEnrollment.IdEnrollment = (int)dr["IdEnrollment"];
                respEnrollment.Semester = (int)dr["Semester"];
                respEnrollment.IdStudy = (int)dr["IdStudy"];
                respEnrollment.StartDate = dr["StartDate"].ToString();
                #endregion

                dr.Dispose();
                tran.Commit();
            }
            return response;
        }

        public PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request)
        {
            if (request.Studies == null || request.Semester == 0)
            {
                throw new ArgumentNullException("Incorrect request");
            }
            PromoteStudentsResponse response = null;
            Enrollment respEnrollment = new Enrollment();

            using (SqlConnection conn = new SqlConnection(ConnString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("PromoteStudents", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@StudyName", request.Studies));
                cmd.Parameters.Add(new SqlParameter("@Semester", request.Semester));

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        cmd.Dispose();
                        throw new ArgumentException("Nothing to be read by SqlDataReader");
                    }
                    response = new PromoteStudentsResponse();
                    response.Enrollment = respEnrollment;
                    respEnrollment.IdEnrollment = (int)dr["IdEnrollment"];
                    respEnrollment.Semester = (int)dr["Semester"];
                    respEnrollment.IdStudy = (int)dr["IdStudy"];
                    respEnrollment.StartDate = dr["StartDate"].ToString();
                }
                cmd.Dispose();
            }
            return response;
        }

        #endregion


        public void SaveLogData(IEnumerable<string> logData)
        {
            string logFilePath = "/Users/azyl/Git-Uni//Tutorial5/requestsLog.txt";
            try
            {
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    foreach (string data in logData)
                    {
                        writer.WriteLine(data);
                    }
                }
            }
            catch (Exception) { }
        }

    }

}