using System;
using System.ComponentModel.DataAnnotations;

namespace Tutorial5.Models
{
    public class Student
    {
        [Required(ErrorMessage="This fied is required")]
        
        [MaxLength(10)]
        
        public string IndexNumber { get; set; }

        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string BirthDate { get; set; }

        public int IdEnrollment { get; set; }

    }
}
