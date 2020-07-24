namespace AdventureRoller.Services
{
    using AdventureRoller.Models;
    using System;
    using System.Collections.Generic;

    public interface ICharacterAttributeService
    {
        Response AddCharacterAttribute(Guid characterId, int level, string attribute, string value);

        Response AddCharacterAttributes(Guid characterId, int level, Dictionary<string, string> attributes);

        StringResponse GetCharacterAttribute(Guid characterId, int level, string attribute);

        Response UpdateCharacterAttributes(Guid characterId, int level, Dictionary<string, string> attributes);

        Response DeleteCharacterAttributes(Guid characterId, int? level = null);
    }
}
