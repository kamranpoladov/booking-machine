using System.ComponentModel.DataAnnotations;

namespace BookingMachine.ViewModels
{
    public class RegistrationViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}