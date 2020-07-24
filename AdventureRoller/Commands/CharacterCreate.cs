using Discord.Commands;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using AdventureRoller.Services;
using System;

namespace AdventureRoller.Commands
{
    public class CharacterCreate : ModuleBase<SocketCommandContext>
    {
        private ICharacterService CharacterService { get; }

        public CharacterCreate(ICharacterService characterService)
        {
            CharacterService = characterService;
        }

        [Command("registercharacter")]
        public async Task Setup()
        {
            await ReplyAsync(new Help().GetHelpInfo("registercharacter"));
        }

        [Command("registercharacter")]
        public async Task Setup(string edition, string name, string level, [Remainder]string schema)
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

                var response = CharacterService.CreateCharacter(discordId, name, edition, level2, values);

                if (response.Success)
                {
                    await ReplyAsync($"{name} registered!");
                }
                else
                {
                    await ReplyAsync($"Registration failed: {response.Error}");
                }

                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                await ReplyAsync("Registration Failed!");

                await Task.CompletedTask;
            }
        }

    }
}
