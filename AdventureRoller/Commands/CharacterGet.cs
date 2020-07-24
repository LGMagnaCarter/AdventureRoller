namespace AdventureRoller.Commands
{
    using Discord.Commands;
    using System.Threading.Tasks;
    using AdventureRoller.Services;
    using System.Linq;

    public class CharacterGet : ModuleBase<SocketCommandContext>
    {
        private ICharacterService CharacterService { get; }

        public CharacterGet(ICharacterService characterService)
        {
            CharacterService = characterService;
        }

        [Command("characters")]
        public async Task Setup()
        {
            var discordId = Context.Message.Author.Id;
            var response = string.Empty;

            var serviceResponse = CharacterService.ListCharacters(discordId);

            if (!serviceResponse.Success)
            {
                await ReplyAsync("Error retrieving characters");
                return;
            }

            if (!serviceResponse.Characters.Any())
            {
                await ReplyAsync("You have no characters. Try using `!registercharacter`");
                return;
            }

            foreach (var character in serviceResponse.Characters.OrderBy(c => c.Edition).ThenBy(c => c.Name).ThenBy(c => c.Level))
            {
                response += $"{character.Edition}\t{character.Level}\t{character.Name}\r\n";
            }

            await ReplyAsync(response);
            return;
        }

        [Command("characters")]
        public async Task Setup(string name, string level)
        {
            var discordId = Context.Message.Author.Id;
            var response = string.Empty;

            if (!int.TryParse(level, out int intLevel) || intLevel < 0)
            {

                await ReplyAsync($"{nameof(level)} has to be a positive integer!");

                await Task.CompletedTask;
                return;
            }

            var characterDetail = CharacterService.GetCharacter(discordId, name, intLevel);

            if (characterDetail == null)
            {
                await ReplyAsync($"Character {name}:{level} does not exist");
                return;
            }

            response += "{\r\n";
            foreach (var attribute in characterDetail.CharacterAttributes.OrderBy(ca => ca.Value))
            {
                response += $"\t\"{attribute.Name}\": \"{attribute.Value}\",\r\n";
            }
            response += "}";

            await ReplyAsync(response);

            await Task.CompletedTask;
        }

    }
}
