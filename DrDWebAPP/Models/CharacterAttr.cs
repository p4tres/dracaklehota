using System.ComponentModel.DataAnnotations;

namespace DrDWebAPP.Models
{
    public class CharacterAttr
    {
        
        [Required(ErrorMessage ="Je potrebne vyplnit toto pole")]
        [RegularExpression(@"^[012]\d/(\+|\-)\d$", ErrorMessage = "Formát musí byť: 0-2 + číslo /+ alebo /- a číslo")]
        public string? CharInteligent { get; set; }

        [Required(ErrorMessage = "Je potrebne vyplnit toto pole")]
        [RegularExpression(@"^[012]\d/(\+|\-)\d$", ErrorMessage = "Formát musí byť: 0-2 + číslo /+ alebo /- a číslo")]
        public string? CharStrenght { get; set; }

        [Required(ErrorMessage = "Je potrebne vyplnit toto pole")]
        [RegularExpression(@"^[012]\d/(\+|\-)\d$", ErrorMessage = "Formát musí byť: 0-2 + číslo /+ alebo /- a číslo")]
        public string? CharAgility { get; set; }

        [Required(ErrorMessage = "Je potrebne vyplnit toto pole")]
        [RegularExpression(@"^[012]\d/(\+|\-)\d$", ErrorMessage = "Formát musí byť: 0-2 + číslo /+ alebo /- a číslo")]
        public string? CharEndurance { get; set; }
        [Required(ErrorMessage = "Je potrebne vyplnit toto pole")]
        [RegularExpression(@"^[012]\d/(\+|\-)\d$", ErrorMessage = "Formát musí byť: 0-2 + číslo /+ alebo /- a číslo")]
        public string? CharCharisma { get; set; }
    }
}
