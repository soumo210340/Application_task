using System.ComponentModel.DataAnnotations;

namespace Application_task.Models
{
    public class UserData
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Hobbies are required.")]
        public string Hobbies { get; set; } // Comma separated
    }
}
