using System.ComponentModel.DataAnnotations;

namespace CompanyEmployees.Entities.DataTransferObjects
{
    public class UserForRegistrationDto
    {
        [Key]
        public string? UserID { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }
        /*
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }

        public string? JobPosition { get; set; }

        public string? TelephoneNo { get; set; }*/

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
