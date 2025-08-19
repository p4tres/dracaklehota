using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DrDWebAPP.Models
{
    public class Dungeon
    {
        [Key]
        public int DunID { get; set; }

        [Required]
        public string? DunName { get; set; }

        public int UserID { get; set; }

    }
}
