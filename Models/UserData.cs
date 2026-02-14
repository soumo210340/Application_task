using System.ComponentModel.DataAnnotations;

namespace Application_task.Models
{
    public class UserData
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid Mobile Number")]
        public string MobileNo { get; set; }

        [Required]


        public string Address { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Hobbies { get; set; } // Comma separated
    }
}
