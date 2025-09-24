using System.ComponentModel.DataAnnotations;

namespace DrDWebAPP.Models.ReadOnly
{
    public class AttributesView
    {
        public string RaceAttributesID { get; set; }
        public string ProfessionAttributesID { get; set; }
        public int StrengthMin { get; set; }
        public int StrengthMax { get; set; }
        public int IntelligenceMin { get; set; }
        public int IntelligenceMax { get;set; }
        public int EnduranceMin { get; set; }
        public int EnduranceMax { get; set; }
        public int DexterityMin { get; set; }
        public int DexterityMax { get; set; }
        public int CharismaMin { get; set; }
        public int CharismaMax { get;set;}
    }
}
