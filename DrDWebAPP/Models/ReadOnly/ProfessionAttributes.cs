using System.ComponentModel.DataAnnotations;

namespace DrDWebAPP.Models.ReadOnly
{
    public class ProfessionAttributes
    {
        [Key] public string ID { get; set; }
        public string Strength { get; set; }
        public string Intelligence { get; set; }
        public string Endurance { get; set; }
        public string Dexterity { get; set; }
        public string Charisma { get; set; }
    }
}
