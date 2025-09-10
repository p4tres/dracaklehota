using System.ComponentModel.DataAnnotations;

namespace DrDWebAPP.Models
{
    public class CharacterInfo
    {
        //[Key] public int CharacterId { get; set; }

        [Required(ErrorMessage = "Meno postavy je povinne")]
        public string CharName { get; set; }

        [Required(ErrorMessage = "Rasa postavy je povinna")]
        public string CharRace { get; set; }

        [Required(ErrorMessage = "Povolanie postavy je povinne")]
        public string CharProfession { get; set; }

        [Range(1, 36, ErrorMessage = "Level postavy musi byt medzi 1-36")]
        public int CharLevel { get; set; }

        [Required(ErrorMessage = "Skusenosti su povinne")]
        [Range(0, int.MaxValue, ErrorMessage = "Skusenosti musia byt nezaporne")]
        public int CharExperiencePoints { get; set; }

        [Required(ErrorMessage = "Maximalne zivoty su povinne")]
        [Range(1, 999, ErrorMessage = "Nemozes mat menej ako 0 maximalnych zivotov")]
        public int CharHitPointsMax { get; set; }
        //public int? CharHitPoints { get; set; }

        [Range(0, 999, ErrorMessage = "Mana musi byt nezaporna, alebo 0")]
        public int CharManaMax { get; set; }
    }
}
