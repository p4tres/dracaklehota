using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DrDWebAPP.Models
{
    public class User
    {
        public int UserId { get; set; }
        [DisplayName("Meno")]
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Surname { get; set; }
        [Required]
        public string? Nickname { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
