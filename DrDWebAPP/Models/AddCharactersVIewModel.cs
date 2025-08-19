namespace DrDWebAPP.Models
{
    public class AddCharactersViewModel
    {
        public int DunID { get; set; }

        public List<Character> AvailableCharacters { get; set; }

        public List<int> SelectedCharacterIds { get; set; }
    }
}

