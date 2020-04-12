using System;
using Tutorial5.Models;
using Tutorial5.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Tutorial5.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private IDbService _service;
    
        public StudentsController(IDbService service)
        {
            this._service = service;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(_service.GetStudents());
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetEnrollment(string indexNumber)
        {
            return Ok(_service.GetEnrollOfStudByIndNo(indexNumber));
        }

    }
}
