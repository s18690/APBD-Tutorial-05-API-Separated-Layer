using System;
using System.ComponentModel.DataAnnotations;

namespace Tutorial5.DTOs.Requests
{
    /*
{
    "IndexNumber": "s12345",
    "FirstName": "Wojtek",
    "LastName": "Dudzinski",
    "BirthDate": "30.03.1993",
    "Studies": "Informatyka dzienne"
}
    */

    public class EnrollStudentRequest
    {

        [Required]
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public string BirthDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string Studies { get; set; }

    }
}