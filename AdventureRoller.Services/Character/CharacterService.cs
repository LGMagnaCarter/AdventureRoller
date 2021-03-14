namespace AdventureRoller.Services
{
    using AdventureRoller.DatabaseContext;
    using AdventureRoller.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class CharacterService : ICharacterService
    {
        private AdventurerollerdbContext DbContext { get; }
        private ICharacterAttributeService CharacterAttributeService { get; }

        private Regex WordRegex = new Regex("[A-z]{3,15}");

        public CharacterService(ICharacterAttributeService characterAttributeService, AdventurerollerdbContext dbContext)
        {
            CharacterAttributeService = characterAttributeService;
            DbContext = dbContext;
        }

        public Response CreateCharacter(ulong discordId, string name, string edition, int level, Dictionary<string, string> attributes)
        {
            //todo: attribute validity service

            DeactivateActiveCharacters(discordId);

            var characterId = Guid.NewGuid();

            DbContext.Characters.Add(new Characters
            {
                CharacterId = characterId,
                Active = true,
                DiscordId = discordId,
                Name = name,
                Edition = edition,
                Level = level
            });
            DbContext.SaveChanges();

            return CharacterAttributeService.AddCharacterAttributes(characterId, level, attributes);
        }

        public ListCharactersResponse ListCharacters(ulong discordId)
        {
            return new ListCharactersResponse(true, DbContext.Characters.AsQueryable().Where(c => c.DiscordId == discordId).ToList());
        }

        public Characters GetCharacter(ulong discordId, string name = null, int? level = null)
        {
            Characters character;
            if (string.IsNullOrWhiteSpace(name))
            {
                character = DbContext.Characters.Include(c => c.CharacterAttributes).AsQueryable().FirstOrDefault(c => c.DiscordId == discordId && c.Active);
            }
            else
            {
                character = DbContext.Characters.Include(c => c.CharacterAttributes).AsQueryable().FirstOrDefault(c => c.DiscordId == discordId && c.Name == name && c.Level == level);
            }

            return character.CharacterId != null ? character : null;
        }

        public StringResponse GetAttribute(ulong discordId, string attribute)
        {
            var character = DbContext.Characters.FirstOrDefault(c => c.Active == true && c.DiscordId == discordId);

            var attributeResponse = CharacterAttributeService.GetCharacterAttribute(character.CharacterId, character.Level, attribute);

            if (!attributeResponse.Success)
            {
                return attributeResponse;
            }

            //todo: add error handling for stringresponse

            var characterAttribute2 = attributeResponse;

            foreach(Match match in WordRegex.Matches(characterAttribute2.Value))
            {

                var subAttributeResponse = CharacterAttributeService.GetCharacterAttribute(character.CharacterId, character.Level, match.Value);

                if (!subAttributeResponse.Success)
                {
                    return subAttributeResponse;
                }

                var regex = new Regex(Regex.Escape(match.Value));
                attributeResponse.Value = regex.Replace(attributeResponse.Value, subAttributeResponse.Value, 1);
            }

            return attributeResponse;
        }

        public Response UpdateCharacter(ulong discordId, string name, int level, Dictionary<string, string> attributes)
        {
            var characterList = DbContext.Characters.AsQueryable().Where(c => c.DiscordId == discordId && c.Name == name && c.Level == level);

            var error = ValidateOnlyOne(characterList, name);

            if (!string.IsNullOrEmpty(error))
            {
                return new StringResponse(false, error);
            }

            return CharacterAttributeService.UpdateCharacterAttributes(characterList.First().CharacterId, level, attributes);
        }

        public Response DeleteCharacter(ulong discordId, string name, int? level = null)
        {
            try
            {
                var characterList = DbContext.Characters.AsQueryable().Where(c => c.DiscordId == discordId && c.Name == name && (!level.HasValue || c.Level == level.Value));

                foreach (var character in characterList)
                {
                    CharacterAttributeService.DeleteCharacterAttributes(character.CharacterId, level);
                    DbContext.Remove(character);
                }

                DbContext.SaveChanges();
            }
            catch(Exception)
            {
                DbContext.RejectChanges();
                return new Response(false, "Error deleting character");
            }

            return Response.SuccessfulResponse;
        }

        public Response SwitchCharacter(ulong discordId, string name, int level)
        {
            var character = DbContext.Characters.AsQueryable().FirstOrDefault(c => c.DiscordId == discordId && c.Name == name);

            if (character.CharacterId == Guid.Empty)
            {
                return new Response(false, "Character does not exist!");
            }

            DeactivateActiveCharacters(discordId);

            character.Active = true;
            character.Level = level;
            DbContext.SaveChanges();

            return Response.SuccessfulResponse;
        }

        private void DeactivateActiveCharacters(ulong discordId)
        {
            var characters = DbContext.Characters.AsQueryable().Where(c => c.DiscordId == discordId);

            if (characters.ToList().Count() == 0)
            {
                return;
            }

            foreach (var character in characters)
            {
                character.Active = false;
            }
        }

        private string ValidateOnlyOne(IQueryable<Characters> cList, string name)
        {
            if(!cList.Any())
            {
                return $"you do not have a character named {name}";
            }

            return string.Empty;
        }
    }
}
