using System.ComponentModel.DataAnnotations;

namespace DrDWebAPP.Models
{
    public class UserModel : User
    {
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Hesla sa nezhoduju!")]
        public string? ConfirmPassword { get; set; }
    }
}
