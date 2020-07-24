using AdventureRoller.DatabaseContext;
using System.Collections.Generic;

namespace AdventureRoller.Models
{
    public class ListCharactersResponse : Response
    {
        public List<Characters> Characters { get; set; }

        public ListCharactersResponse(bool success, List<Characters> characters, string error = "") : base(success, error)
        {
            Characters = characters;
        }

        public ListCharactersResponse(bool success, string error): base(success, error) {}
    }
}
