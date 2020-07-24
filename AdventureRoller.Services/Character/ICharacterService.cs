namespace AdventureRoller.Services
{
    using AdventureRoller.DatabaseContext;
    using AdventureRoller.Models;
    using System.Collections.Generic;

    public interface ICharacterService
    {
        Response CreateCharacter(ulong discordId, string name, string edition, int level, Dictionary<string, string> attributes);

        Response SwitchCharacter(ulong discordId, string name, int level);

        StringResponse GetAttribute(ulong discordId, string attribute);

        Response UpdateCharacter(ulong discordId, string name, int level, Dictionary<string, string> attributes);

        ListCharactersResponse ListCharacters(ulong discordId);

        Characters GetCharacter(ulong discordId, string name, int level);

        Response DeleteCharacter(ulong discordId, string name, int? level = null);
    }
}
