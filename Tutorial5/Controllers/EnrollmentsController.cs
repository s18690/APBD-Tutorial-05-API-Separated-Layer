using System.Diagnostics;
using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Tutorial5.DTOs.Requests;
using Tutorial5.DTOs.Responses;
using Tutorial5.Models;
using Tutorial5.Services;
using Microsoft.AspNetCore.Mvc;


namespace Tutorial5.Controllers
{
    [Route("api/enrollments/")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {

        private IDbService _service;

        public EnrollmentsController(SqlServerDbService service)
        {
            _service = service;
        }
        [HttpPost]
        [ActionName("EnrollStudent")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            try
            {
                var response = _service.EnrollStudent(request);
                return CreatedAtAction("EnrollStudent", response);
            }
            catch (ArgumentException)
            {
                return NotFound("Studies with given name not found");
            }
            catch (InvalidOperationException)
            {
                return BadRequest("Student with given index number already exists");
            }
        }

        [Route("promotions")]
        [HttpPost]
        [ActionName("PromoteStudents")]
        public IActionResult PromoteStudents(PromoteStudentsRequest request)
        {
            try
            {
                var response = _service.PromoteStudents(request);
                return CreatedAtAction("PromoteStudents", response);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Invalid request");
            }
            catch (SqlException)
            {
                return NotFound("Studies with given name not found");
            }
            catch (ArgumentException)
            {
                return BadRequest("No values for response found");
            }
        }
    }
}