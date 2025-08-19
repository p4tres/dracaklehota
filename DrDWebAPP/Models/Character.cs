using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DrDWebAPP.Models
{
    public class Character
    {
        [Key]public int CharacterId { get; set; }

        public string? CharName { get; set; }

        public string? CharRace { get; set; }

        public string? CharProfession { get; set; }

        public int? CharLevel{ get; set; }


        public int? CharExperiencePoints { get; set; }


        public int? CharHitPoints { get; set; }


        public string? CharInteligent { get; set; }


        public string? CharStrenght { get; set; }


        public string? CharAgility { get; set; }


        public string? CharEndurance { get; set; }


        public string? CharCharisma { get; set; }

        public int? CharMana { get; set; }

        public string? CharWeapons { get; set; }

        public string? CharDefense { get; set; }

        public string? CharItems { get; set; }

        public int? UserID { get; set; }
        public int? DunID { get; set; }
    }
}

