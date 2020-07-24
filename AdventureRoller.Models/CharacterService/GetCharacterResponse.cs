namespace AdventureRoller.Models.CharacterService
{
    using AdventureRoller.DatabaseContext;

    class GetCharacterResponse : Response
    {
        public Characters Character { get; set; }

        public GetCharacterResponse(bool success, Characters character, string error = "") : base(success, error)
        {
            Character = character;
        }

        public GetCharacterResponse(bool success, string error) : base(success, error) { }
    }
}
