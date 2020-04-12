using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace Tutorial5.DTOs.Requests
{
    
    public class PromoteStudentsRequest
    {
        [Required]
        [MaxLength(100)]
        public string Studies { get; set; }
        [Required]
        public int Semester { get; set; }
    }
}