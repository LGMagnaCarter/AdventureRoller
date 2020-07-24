using Discord.Commands;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using AdventureRoller.Services;
using System;

namespace AdventureRoller.Commands
{
    public class CharacterUpdate : ModuleBase<SocketCommandContext>
    {
        private ICharacterService CharacterService { get; }

        public CharacterUpdate(ICharacterService characterService)
        {
            CharacterService = characterService;
        }

        [Command("updatecharacter")]
        public async Task Setup(string name, string level, [Remainder]string schema)
        {
            try
            {
                Dictionary<string, string> values;
                try
                {
                    values = JsonConvert.DeserializeObject<Dictionary<string, string>>(schema);
                }
                catch (Exception e)
                {
                    await ReplyAsync("Invalid Format!");

                    await Task.CompletedTask;
                    return;
                }

                if (!int.TryParse(level, out int level2) || level2 < 0)
                {

                    await ReplyAsync($"{nameof(level)} has to be a positive integer!");

                    await Task.CompletedTask;
                    return;
                }

                var discordId = Context.Message.Author.Id;

                var response = CharacterService.UpdateCharacter(discordId, name, level2, values);

                if (response.Success)
                {
                    await ReplyAsync($"{name} updated!");
                }
                else
                {
                    await ReplyAsync($"update failed! {response.Error}");
                }

                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                await ReplyAsync("Update Failed!");

                await Task.CompletedTask;
            }
        }

    }
}
