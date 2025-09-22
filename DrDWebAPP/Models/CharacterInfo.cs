using System.ComponentModel.DataAnnotations;

namespace DrDWebAPP.Models
{
    public class CharacterInfo
    {
        //[Key] public int CharacterId { get; set; }

        [Required(ErrorMessage = "Meno postavy je povinné")]
        public string CharName { get; set; }

        [Required(ErrorMessage = "Rasa postavy je povinná")]
        public string CharRace { get; set; }

        [Required(ErrorMessage = "Povolanie postavy je povinné")]
        public string CharProfession { get; set; }

        [Range(1, 36, ErrorMessage = "Level postavy musí byť medzi 1-36")]
        public int CharLevel { get; set; }

        [Required(ErrorMessage = "Skúsenosti sú povinné, vždy treba zadať číselnú hodnotu")]
        [Range(0, int.MaxValue, ErrorMessage = "Skúsenosti musia byť 0/viac")]
        public int CharExperiencePoints { get; set; }

        [Required(ErrorMessage = "Maximálne životy sú povinné")]
        [Range(1, 999, ErrorMessage = "Nemozes mat menej ako 0 maximalnych zivotov")]
        public int CharHitPointsMax { get; set; }
        //public int? CharHitPoints { get; set; }
        [Required(ErrorMessage ="Manu je potrebné zadať,treba zadať číselnú hodnotu ")]
        [Range(0, 999, ErrorMessage = "Mana musí byť nezáporná, alebo 0")]
        public int CharManaMax { get; set; }
    }
}
