using Discord.Commands;
using System.Threading.Tasks;
using AdventureRoller.Services;

namespace AdventureRoller.Commands
{
    public class CharacterChange : ModuleBase<SocketCommandContext>
    {

        private ICharacterService CharacterService { get; }

        public CharacterChange(ICharacterService characterService)
        {
            CharacterService = characterService;
        }

        [Command("changecharacter")]
        public async Task Setup(string name, int level)
        {
            var response = CharacterService.SwitchCharacter(Context.Message.Author.Id, name, level);

            if(response.Success)
            {
                await ReplyAsync($"switched to character {name} : {level}");
            }
            else
            {
                await ReplyAsync($"Errror switching character: {response.Error}");
            }
        }

    }
}
