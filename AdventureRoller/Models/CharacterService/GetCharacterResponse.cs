namespace AdventureRoller.Models.CharacterService
{
    using AdventureRoller.Commands;

    class GetCharacterResponse : Response
    {
        public CharacterGet Character { get; set; }

        public GetCharacterResponse(bool success, CharacterGet character, string error = "") : base(success, error)
        {
            Character = character;
        }

        public GetCharacterResponse(bool success, string error) : base(success, error) { }
    }
}
