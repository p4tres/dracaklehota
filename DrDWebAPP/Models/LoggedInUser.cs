using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DrDWebAPP.Models
{
    public class LoggedInUser
    {
        [Required] public string? UserName { get; set; }
        [Required] public string? Password { get; set; }
    }
}

